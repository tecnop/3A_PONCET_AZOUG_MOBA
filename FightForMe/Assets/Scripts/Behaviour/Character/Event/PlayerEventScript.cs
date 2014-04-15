using UnityEngine;
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
		if (buffID == 4)
		{ // Just got the trophy, recover our health and mana (and cooldowns if we decide to implement them)
			_manager.GetStatsScript().Restore();
		}
	}

	public override void OnKnockback(CharacterManager inflictor)
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

		if (_manager.GetStatsScript().HasSpecialEffect(MiscEffect.CARRYING_TROPHY))
		{ // Respawn the monster
			HasnorSpawnerScript spawner = FindObjectOfType<HasnorSpawnerScript>();
			if (spawner)
			{
				spawner.TryRespawn();
			}
			else
			{
				Debug.LogError("Could not respawn Hasnor!");
			}
		}

		_manager.GetMiscDataScript().GetSpawner().OnSpawnedEntityDeath();

		_manager.GetCombatScript().RemoveBuffs();
	}

	public override void OnSpotEntity(GameObject entity)
	{
		//Debug.Log(_manager.name + " spotted " + entity.name);
		//Debug.DrawLine(_manager.GetCharacterTransform().position, entity.transform.position, Color.white, 3.0f);
	}

	public override void OnLoseSightOfEntity(GameObject entity)
	{

	}

	public override void OnCollision(Collider collider)
	{

	}
}
