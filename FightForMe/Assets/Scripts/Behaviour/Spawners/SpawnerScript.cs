using UnityEngine;
using System.Collections;

public abstract class SpawnerScript : MonoBehaviour
{
	public abstract void OnSpawnedEntityDeath();
	public abstract void Spawn();
}
