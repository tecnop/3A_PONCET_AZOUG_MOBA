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

	public void SetSpawner(MonsterSpawnerScript spawner)
	{
		this.spawner = spawner;
		this.spawnerPos = spawner.transform.position;
	}

	public MonsterSpawnerScript GetSpawner()
	{
		return this.spawner;
	}

	public Vector3 GetSpawnPos()
	{
		return this.spawnerPos;
	}
}
