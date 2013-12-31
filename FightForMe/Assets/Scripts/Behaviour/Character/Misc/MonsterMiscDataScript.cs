using UnityEngine;
using System.Collections;

public class MonsterMiscDataScript : CharacterMiscDataScript
{
	private GameObject spawner;			// Entity that spawned us
	private Vector3 spawnerPos;

	public void SetSpawner(GameObject spawner)
	{
		this.spawner = spawner;
		this.spawnerPos = spawner.transform.position;
	}

	public Vector3 GetSpawnPos()
	{
		return this.spawnerPos;
	}
}
