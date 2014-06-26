using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEventScript : CharacterEventScript
{
	private PlayerInputScript _input;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;

		_input = (PlayerInputScript)_manager.GetInputScript();
	}

	public override void OnPain(CharacterManager inflictor, float damage)
	{
		if (damage > 0)
		{
			_manager.GetCharacterAnimator().SetBool("onPain", true);
		}
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

		List<DamageInstance> combatLog = _manager.GetCombatScript().GetCombatLog();
		((PlayerSpawnerScript)_manager.GetMiscDataScript().GetSpawner()).SetLog(combatLog);

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
		_manager.GetMovementScript().SetMovementOverride(Vector3.zero, 0.0f, 0, false);
	}

	public override void OnSpotEntity(VisibleEntity entity)
	{
		if (_manager == GameData.activePlayer)
		{
			entity.SetVisible(true);
		}
		_input.NotifyEntityNoticed(entity);
	}

	public override void OnLoseSightOfEntity(VisibleEntity entity)
	{
		if (_manager == GameData.activePlayer)
		{
			entity.SetVisible(false);
		}
		_input.NotifyEntityLost(entity);
	}

	public override void OnCollision(Collider collider)
	{

	}
}
