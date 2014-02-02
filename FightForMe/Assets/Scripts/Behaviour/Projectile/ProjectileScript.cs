using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour
{
	//private float damageAmplifier;		// Percentage of damage to inflict on impact (default: 1.0f)
	private float damage;				// Damage to inflict on impact
	private float damageRadius;			// Radius of the area in which to inflict damage
	private uint buffID;				// Index of the entry in the buff table that should be inflicted upon collision
	private float buffDuration;			// Duration of the buff

	private CharacterManager owner;		// Character responsible for our actions

	private CharacterCombatScript _ownerCombat;
	private Transform _transform;

	void Start()
	{
		_transform = this.transform;
		_ownerCombat = owner.GetCombatScript();
	}

	void OnCollision(Collider collider)
	{
		if (collider.tag == "Player")
		{ // If we collided with it then it's got to be an enemy
			CharacterManager hisManager = collider.GetComponent<CharacterPhysicsScript>().GetManager();

			if (hisManager)
			{
				_ownerCombat.Damage(hisManager, damage, collider.ClosestPointOnBounds(_transform.position), 0);
				_ownerCombat.InflictBuff(hisManager, this.buffID, this.buffDuration);
			}
		}

		// TODO: Radial damage
	}
}
