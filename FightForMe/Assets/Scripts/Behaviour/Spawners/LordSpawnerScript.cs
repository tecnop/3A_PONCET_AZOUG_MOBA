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

	private Vector3 _pos;
	private Quaternion _ang;

	private static Monster lordData;

	void Start()
	{
		if (!GameData.isServer)
		{ // We're a server-only entity
			Destroy(this.gameObject);
			return;
		}

		Transform transform = this.transform;
		_pos = transform.position;
		_ang = transform.rotation;

		_initialized = true;

		if (lordData == null)
		{ // Let's set it up then!
			lordData = DataTables.GetMonster(2);
		}

		Spawn();
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

		if (_team == Team.Team1)
		{
			lord.layer = LayerMask.NameToLayer("Team1Entity");
		}
		else if (_team == Team.Team2)
		{
			lord.layer = LayerMask.NameToLayer("Team2Entity");
		}
		manager.GetPhysicsScript().gameObject.layer = lord.layer;

		manager.MakeLocal(); // If we got here we're the server, so make the monster local
		manager.GetMiscDataScript().SetSpawner(this);

		// Set him the data we got from the data table
		lord.name = lordData.GetName();
		((NPCAIScript)manager.GetInputScript()).SetBehaviour(lordData.GetBehaviour());
		manager.GetInventoryScript().SetItems(lordData.GetItems());

		manager.GetGraphicsLoader().LoadModel(lordData.GetModelPath());
		//manager.GetCharacterTransform().localScale *= monster.GetScale();
	}

	public override void OnSpawnedEntityDeath()
	{
		if (_team == Team.Team2)
		{ // Player 1 won
			Debug.Log("Player 1 won!");
			GameData.activePlayer.GetCameraScript().GetHUDScript().SetState(HUDState.Won);
		}
		else if (_team == Team.Team1)
		{ // Player 2 won
			Debug.Log("Player 2 won!");
			GameData.activePlayer.GetCameraScript().GetHUDScript().SetState(HUDState.Lost);
		}

		GameData.gamePaused = true;
	}
}
