using UnityEngine;
using System.Collections;

public class MonsterSpawnerScript : SpawnerScript
{
	[SerializeField] // WHY CAN'T UNSIGNED INTEGERS BE SERIALIZED
	private int[] _monsterList;		// List of monsters we should spawn (index is the level of the camp)

	[SerializeField]
	private GameObject _monsterPrefab;

	private MonsterCampScript camp;	// Camp we're a part of

	private Vector3 _pos;
	private Quaternion _ang;

	private bool _initialized;
	private bool _spawnPending;

	public void LinkToCamp(MonsterCampScript camp)
	{
		this.camp = camp;
	}

	void Start()
	{
		if (!GameData.isServer)
		{ // We're a server-only entity
			Destroy(this.gameObject);
			return;
		}

		if (_monsterList.Length == 0)
		{ // We don't have any bound monsters, no reason for us to be here
			Destroy(this.gameObject);
			return;
		}

		Transform transform = this.transform;

		_pos = transform.position;
		_ang = transform.rotation;

		_initialized = true;

		if (!camp || _spawnPending)
		{ // We're not linked to a trigger, we're probably just a one-time spawner then
			Spawn();
		}
	}

	private void DoSpawnMonster(uint monsterID)
	{
		Monster monster = DataTables.GetMonster(monsterID);

		if (monster == null)
		{ // Hmmm...
			Debug.LogWarning("Spawner tried to spawn unknown monster " + monsterID);
			return;
		}

		// Spawn the entity
		GameObject monsterObject;
		if (GameData.isOnline)
		{
			monsterObject = (GameObject)Network.Instantiate(_monsterPrefab, _pos, _ang, 0);
		}
		else
		{
			monsterObject = (GameObject)Instantiate(_monsterPrefab, _pos, _ang);
		}

		// Link us together
		CharacterManager manager = monsterObject.GetComponent<CharacterManager>();
		manager.MakeLocal(); // If we got here we're the server, so make the monster local
		((MonsterMiscDataScript)manager.GetMiscDataScript()).SetSpawner(this);

		// Set him the data we got from the data table
		monsterObject.name = monster.GetName();
		((NPCAIScript)manager.GetInputScript()).SetBehaviour(monster.GetBehaviour());
		manager.GetInventoryScript().SetItems(monster.GetItems());

		manager.GetGraphicsLoader().LoadModel(monster.GetModelPath());
		//manager.GetCharacterTransform().localScale *= monster.GetScale();
	}

	public override void Spawn()
	{
		if (!_initialized)
		{ // Let us initialize first!
			_spawnPending = true;
			return;
		}

		if (_monsterList.Length == 0)
		{ // We don't have any bound monsters, no reason for us to be here (checking again in case dynamic stuff happens)
			Destroy(this.gameObject);
			return;
		}

		uint monsterID;

		if (!camp)
		{ // Just spawn our first entry
			 monsterID = (uint)_monsterList[0];
		}
		else
		{ // Check the level of the camp, and spawn our entry with that index
			if (_monsterList.Length < camp.GetLevel() + 1)
			{
				monsterID = (uint)_monsterList[_monsterList.Length-1];
			}
			else
			{
				monsterID = (uint)_monsterList[camp.GetLevel()];
			}
		}

		DoSpawnMonster(monsterID);
	}

	public override void OnSpawnedEntityDeath()
	{
		if (camp)
		{
			camp.OnBoundMonsterDeath();
		}
		else if ((uint)_monsterList[0] == 2)
		{ // The Lord is dead! (hard-coded for now)
			// Here: Victory sequence
			Debug.Log("Lord died!");
		}
		else
		{ // Kill us or respawn him? I'll go with respawn for now for debugging
			Spawn();
		}
	}
}
