using UnityEngine;
using System.Collections;

public class MonsterMiscDataScript : CharacterMiscDataScript
{
	private MonsterSpawnerScript spawner;			// Entity that spawned us
	private Vector3 spawnerPos;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
	}

	public override void SetSpawner(SpawnerScript spawner)
	{
		this.spawner = (MonsterSpawnerScript)spawner;
		this.spawnerPos = spawner.transform.position;
	}

	public override SpawnerScript GetSpawner()
	{
		return this.spawner;
	}

	public Vector3 GetSpawnPos()
	{
		return this.spawnerPos;
	}
}
