using UnityEngine;
using System.Collections;

public class LordSpawnerScript : SpawnerScript
{
	private Vector3 _pos;

	void Start()
	{
		_pos = this.transform.position;

		_initialized = true;

		if (_spawnPending)
		{
			Spawn();
		}
	}

	public override void Spawn()
	{

	}

	public override void OnSpawnedEntityDeath()
	{
		Debug.Log("Lord is dead!");
	}
}
