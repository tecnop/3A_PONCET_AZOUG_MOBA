using UnityEngine;
using System.Collections;

/*
 * CharacterEventScript.cs
 * 
 * Receives various events from other scripts and executes character type-specific code
 * 
 */

public abstract class CharacterEventScript : MonoBehaviour
{
	protected CharacterManager _manager;

	public abstract void Initialize(CharacterManager manager);

	public abstract void OnPain(CharacterManager inflictor, float damage);
	public abstract void OnReceiveBuff(CharacterManager inflictor, uint buffID);
	public abstract void OnKnockback(CharacterManager inflictor);
	public abstract void OnDeath(CharacterManager killer);
	public abstract void OnSpotEntity(GameObject entity);
	public abstract void OnLoseSightOfEntity(GameObject entity);
	public abstract void OnCollision(Collider collider);
}
