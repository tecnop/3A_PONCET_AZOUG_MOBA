using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
	//private float damageAmplifier;		// Percentage of damage to inflict on impact (default: 1.0f)
	private float damage;				// Damage to inflict on impact
	private float damageRadius;			// Radius of the area in which to inflict damage

	private CharacterManager owner;		// Character responsible for our actions

	private Transform _transform;

	void Start()
	{
		_transform = this.transform;
	}

	void OnCollision(Collider collider)
	{
		if (collider.tag == "Player")
		{ // If we collided with it then it's got to be an enemy
			CharacterManager hisManager = collider.GetComponent<CharacterMovementScript>().GetManager();

			if (hisManager)
			{
				owner.GetCombatScript().Damage(hisManager, damage, collider.ClosestPointOnBounds(_transform.position), 0);
			}
		}

		// TODO: Radial damage
	}
}
