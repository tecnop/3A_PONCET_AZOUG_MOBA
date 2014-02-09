using UnityEngine;
using System.Collections;

public abstract class SpawnerScript : MonoBehaviour
{
	// Spawners tend to be called before they're initialized, so we allow them to queue up spawns
	protected bool _initialized;
	protected bool _spawnPending;

	public abstract void OnSpawnedEntityDeath();
	public abstract void Spawn();
}
