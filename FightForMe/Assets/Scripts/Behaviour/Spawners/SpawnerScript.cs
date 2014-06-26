using UnityEngine;
using System.Collections;

public abstract class SpawnerScript : MonoBehaviour
{
	[SerializeField]
	protected Transform _transform;

	// Spawners tend to be called before they're initialized, so we allow them to queue up spawns
	protected bool _initialized;
	protected bool _spawnPending;

	public abstract void OnSpawnedEntityDeath();
	public abstract CharacterManager Spawn();

	public Transform GetTransform()
	{
		return _transform;
	}
}
