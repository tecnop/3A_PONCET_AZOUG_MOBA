using UnityEngine;
using System.Collections;

public abstract class CharacterEventScript : MonoBehaviour
{
	[SerializeField]
	protected CharacterManager _manager;

	public abstract void OnPain(float damage);
	public abstract void OnDeath(CharacterManager killer);
	public abstract void OnSpotEntity(GameObject entity);
	public abstract void OnCollision(Collider collider);
}
