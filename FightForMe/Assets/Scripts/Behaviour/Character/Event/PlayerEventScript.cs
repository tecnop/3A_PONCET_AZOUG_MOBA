using UnityEngine;
using System.Collections;

public class PlayerEventScript : CharacterEventScript
{
	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
	}

	public override void OnPain(float damage)
	{

	}

	public override void OnDeath(CharacterManager killer)
	{

		_manager.GetCharacterAnimator().SetBool("isDead", true);

		//_manager.GetMiscDataScript().GetSpawner().OnSpawnedEntityDeath();
	}

	public override void OnSpotEntity(GameObject entity)
	{

	}

	public override void OnCollision(Collider collider)
	{ // Temporary debug stuff
		if (collider.tag == "DroppedItem")
		{
			DroppedItemScript item = collider.GetComponent<DroppedItemScript>();
			item.OnPickUp(_manager.GetInventoryScript());
		}
	}
}
