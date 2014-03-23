using UnityEngine;
using System.Collections;

public enum Team
{
	Team1,
	Team2
}

public class LordSpawnerScript : SpawnerScript
{
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
		Transform transform = this.transform;
		_pos = transform.position;
		_ang = transform.rotation;

		_initialized = true;

		if (GameData.isServer)
		{
			Spawn();
		}
	}

	public override void Spawn()
	{
		GameObject lord;
		if (GameData.isOnline)
		{
			lord = (GameObject)Network.Instantiate(_monsterPrefab, _pos, _ang, 0);
		}
		else
		{
			lord = (GameObject)Instantiate(_monsterPrefab, _pos, _ang);
		}
		SetUpLord(lord);
	}

	private void SetUpLord(GameObject lord)
	{
		CharacterManager manager = lord.GetComponent<CharacterManager>();
		int layer;

		if (_team == Team.Team1)
		{
			layer = LayerMask.NameToLayer("Team1Entity");
		}
		else
		{
			layer = LayerMask.NameToLayer("Team2Entity");
		}

		manager.GetPhysicsScript().SetLayer(layer);

		MonsterMiscDataScript misc = (MonsterMiscDataScript)manager.GetMiscDataScript();

		misc.SetSpawner(this);

		misc.SetUpFromMonster(1); // HARD CODED REFERENCE
	}

	public override void OnSpawnedEntityDeath()
	{
		if (GameData.isOnline)
		{
			_networkView.RPC("GameOver", RPCMode.All);
		}
		else
		{
			GameOver();
		}
	}

	[RPC]
	private void GameOver()
	{
		if (_team == Team.Team2)
		{ // Player 1 won
			Debug.Log("Player 1 won!");
			//GameData.activePlayer.GetCameraScript().GetHUDScript().SetState(HUDState.Won);
			HUDRenderer.SetState(HUDState.Won);
		}
		else if (_team == Team.Team1)
		{ // Player 2 won
			Debug.Log("Player 2 won!");
			//GameData.activePlayer.GetCameraScript().GetHUDScript().SetState(HUDState.Lost);
			HUDRenderer.SetState(HUDState.Lost);
		}

		GameData.gamePaused = true;
	}
}
