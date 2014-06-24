using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterCampScript : MonoBehaviour
{
	public static List<MonsterCampScript> camps; // Whatever, kind of in a hurry

	private List<MonsterSpawnerScript> _spawners;	// Spawners that are part of this camp (type: MonsterSpawnerScript)

	private uint currentLevel;		// Current level of the camp (goes up each time all the monsters die)
	private uint stillAlive;		// Monsters still alive

	private Vector3 pos;

	public bool respawning
	{
		get
		{
			return stillAlive == 0;
		}
	}

	void Start()
	{
		if (!GameData.isServer)
		{ // We're a server-only entity
			Destroy(this.gameObject);
			return;
		}

		_spawners = new List<MonsterSpawnerScript>(this.GetComponentsInChildren<MonsterSpawnerScript>());

		if (_spawners.Count == 0)
		{ // No reason to be here...
			Destroy(this.gameObject);
			return;
		}

		pos = this.transform.position;

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
			try
			{
				spawner.Spawn();
				stillAlive++;
			}
			catch (System.Exception e)
			{
				Debug.LogWarning(e.Message);
			}
		}

		if (stillAlive == 0)
		{ // Oh, okay. Level us up so this hopefully doesn't happen next wave
			currentLevel++;
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
			Debug.LogWarning(this.name + " was not expecting any more death events");
			return;
		}

		stillAlive--;
		if (stillAlive == 0)
		{
			currentLevel++;
		}
	}

	public Vector3 GetPos()
	{
		return pos;
	}
}
