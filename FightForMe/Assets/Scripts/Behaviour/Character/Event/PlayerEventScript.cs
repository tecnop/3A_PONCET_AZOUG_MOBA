﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEventScript : CharacterEventScript
{
	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
	}

	public override void OnPain(CharacterManager inflictor, float damage)
	{
		_manager.GetCharacterAnimator().SetBool("onPain", true);
	}

	public override void OnReceiveBuff(CharacterManager inflictor, uint buffID)
	{
		
	}

	public override void OnDeath(CharacterManager killer)
	{
		// Reseting states
		_manager.GetCharacterAnimator().SetBool("isAttacking", false);
		_manager.GetCharacterAnimator().SetBool("onPain", false);

		_manager.GetCharacterAnimator().SetBool("isDead", true);

		// DEBUG
		List<DamageInstance> combatLog = _manager.GetCombatScript().GetCombatLog();
		foreach (DamageInstance log in combatLog)
		{
			Debug.Log(log);
		}

		_manager.GetMiscDataScript().GetSpawner().OnSpawnedEntityDeath();
		
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
