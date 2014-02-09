using UnityEngine;
using System.Collections;

/*
 * CharacterMiscScript.cs
 * 
 * Stores miscellaneous data specific to the kind of character we are
 * 
 */

public abstract class CharacterMiscDataScript : MonoBehaviour
{
	protected CharacterManager _manager;

	protected SpawnerScript _spawner;
	protected Vector3 _spawnerPos;

	public abstract void Initialize(CharacterManager manager);

	public void SetSpawner(SpawnerScript spawner)
	{
		this._spawner = spawner;
		this._spawnerPos = spawner.transform.position;
	}

	public SpawnerScript GetSpawner()
	{
		return this._spawner;
	}

	public Vector3 GetSpawnPos()
	{
		return this._spawnerPos;
	}
}
