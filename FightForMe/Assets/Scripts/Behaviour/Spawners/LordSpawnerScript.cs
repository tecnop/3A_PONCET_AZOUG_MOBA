using UnityEngine;
using System.Collections;

public enum Team
{
	Team1,
	Team2
}

public class LordSpawnerScript : SpawnerScript
{
	// Rushed static variables for my AI
	private static CharacterManager _team1Lord;
	public static CharacterManager team1Lord
	{
		get
		{
			return _team1Lord;
		}
	}

	private static CharacterManager _team2Lord;
	public static CharacterManager team2Lord
	{
		get
		{
			return _team2Lord;
		}
	}


	[SerializeField]
	private GameObject _monsterPrefab;

	[SerializeField]
	private Team _team;

	[SerializeField]
	private NetworkView _networkView;

	private Vector3 _pos;
	private Quaternion _ang;

	void Start()
	{
		_pos = _transform.position;
		_ang = _transform.rotation;

		_initialized = true;

		if (GameData.isServer)
		{
			Spawn();
		}
	}

	public override CharacterManager Spawn()
	{
		GameObject lord;
		if (GameData.isOnline)
		{
			lord = (GameObject)Network.Instantiate(_monsterPrefab, _pos, _ang, 1);
		}
		else
		{
			lord = (GameObject)Instantiate(_monsterPrefab, _pos, _ang);
		}

		return SetUpLord(lord);
	}

	private CharacterManager SetUpLord(GameObject lord)
	{
		CharacterManager manager = lord.GetComponent<CharacterManager>();
		int layer;

		if (_team == Team.Team1)
		{
			layer = LayerMask.NameToLayer("Team1Entity");
			_team1Lord = manager;
		}
		else
		{
			layer = LayerMask.NameToLayer("Team2Entity");
			_team2Lord = manager;
		}

		manager.GetPhysicsScript().SetLayer(layer);

		MonsterMiscDataScript misc = (MonsterMiscDataScript)manager.GetMiscDataScript();

		misc.SetSpawner(this);

		misc.SetUpFromMonster(1); // HARD CODED REFERENCE

		if (GameData.gameMode != GameMode.KillTheLord)
		{ // You can't kill him in any other mode
			if (GameData.isOnline)
			{
				_networkView.RPC("MakeInvulnerable", RPCMode.AllBuffered);
			}
			else
			{
				MakeInvulnerable();
			}
		}

		return manager;
	}

	[RPC]
	private void MakeInvulnerable()
	{
		if (_team == Team.Team1)
		{
			_team1Lord.GetCombatScript().ReceiveBuff(_team1Lord, 7);
		}
		else
		{
			_team2Lord.GetCombatScript().ReceiveBuff(_team2Lord, 7);
		}
	}

	public override void OnSpawnedEntityDeath()
	{
		if (GameData.gameMode == GameMode.KillTheLord)
		{
			if (GameData.isOnline)
			{
				_networkView.RPC("GameOver", RPCMode.AllBuffered);
			}
			else
			{
				GameOver();
			}
		}
	}

	[RPC]
	private void GameOver()
	{
		if (_team == Team.Team2)
		{ // Player 1 won
			GameData.pauseMessage = PauseMessage.PLAYER1_VICTORY;
		}
		else if (_team == Team.Team1)
		{ // Player 2 won
			GameData.pauseMessage = PauseMessage.PLAYER2_VICTORY;
		}

		GameData.gamePaused = true;
	}
}
