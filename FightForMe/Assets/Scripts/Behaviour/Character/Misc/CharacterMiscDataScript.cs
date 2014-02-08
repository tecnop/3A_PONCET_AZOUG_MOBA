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

	public abstract void Initialize(CharacterManager manager);

	public abstract void SetSpawner(SpawnerScript spawner);
	public abstract SpawnerScript GetSpawner();
}
