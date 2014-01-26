using UnityEngine;
using System.Collections;

public class MonsterCampScript : MonoBehaviour
{
	private MonsterSpawnerScript[] _spawners;	// Spawners that are part of this camp

	private uint currentLevel;					// Current level of the camp (goes up each time all the monsters die)
	private uint stillAlive;					// Monsters still alive

	void Start()
	{
		_spawners = this.GetComponentsInChildren<MonsterSpawnerScript>();

		if (_spawners.Length == 0)
		{ // No reason to be here...
			Destroy(this.gameObject);
			return;
		}

		foreach (MonsterSpawnerScript spawner in _spawners)
		{
			spawner.LinkToCamp(this);
		}

		currentLevel = 0;
		stillAlive = 0;
	}

	private void Spawn()
	{
		foreach (MonsterSpawnerScript spawner in _spawners)
		{
			spawner.Spawn();
			stillAlive++;
		}
	}

	public void TrySpawn()
	{
		if (stillAlive == 0)
		{
			Spawn();
		}
	}

	public uint GetLevel()
	{
		return this.currentLevel;
	}

	public void OnBoundMonsterDeath()
	{
		if (stillAlive <= 0)
		{
			Debug.Log("ERROR: Camp was not expecting any more death events");
			return;
		}

		stillAlive--;
		if (stillAlive == 0)
		{
			currentLevel++;
		}
	}
}
