using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * CharacterCombatScript.cs
 * 
 * Allows characters to "interact" with each other.
 * 
 */

public class CharacterCombatScript : MonoBehaviour
{
	[SerializeField]
	private GameObject damageSpherePrefab;

	[SerializeField]
	private GameObject projectilePrefab;

	private CharacterManager _manager;

	private Transform _transform;

	private List<InflictedBuff> buffs;		// List of active buffs and debuffs (type: InflictedBuff)
	// TODO: Add a list of effects built from the list of buffs for quicker access? Would building it be more expensive than accessing each entry individually...?

	private List<DamageInstance> combatLog;	// List of previously applied damage instances, cleared upon respawn (type: DamageInstance)

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = this.transform;

		if (buffs == null)
		{
			buffs = new List<InflictedBuff>();
		}

		if (combatLog == null)
		{
			combatLog = new List<DamageInstance>();
		}
	}

	public void Damage(CharacterManager target, float damage)
	{
		target.GetStatsScript().LoseHealth(_manager, damage);
	}

	public HitboxScript CreateAoE(Vector3 position, Quaternion angle, float radius, uint collisionSpellID, float duration = 0.2f, bool makeParent = false, bool particles = false)
	{
		GameObject sphere = (GameObject)Instantiate(damageSpherePrefab, position, angle);
		HitboxScript sphereScript = sphere.GetComponent<HitboxScript>();
		sphereScript.SetUp(_manager, radius, _manager.GetLayer(), collisionSpellID, duration, makeParent, particles);
		return sphereScript;
	}

	public HitboxScript CreateAoE(float radius, uint collisionSpellID, float duration = 0.2f, bool particles = false)
	{
		return CreateAoE(_transform.position, _transform.rotation, radius, collisionSpellID, duration: duration, makeParent: true, particles: particles);
	}

	public ProjectileScript CreateProjectile(uint projectileID, Vector3 position, Quaternion angle, uint impactSpellOverride = 0)
	{ // TODO: Take a position rather than an angle so we can throw projectiles directly there
		GameObject proj = (GameObject)Instantiate(projectilePrefab, position, angle);
		ProjectileScript projScript = proj.GetComponent<ProjectileScript>();
		projScript.SetUp(_manager, projectileID, impactSpellOverride);
		return projScript;
	}

	public ProjectileScript CreateProjectile(uint projectileID, uint impactSpellOverride = 0)
	{
		return CreateProjectile(projectileID, _transform.position, _transform.rotation, impactSpellOverride);
	}

	public void RemoveBuff(uint buffID)
	{
		int i = 0;
		while (i < this.buffs.Count)
		{
			if (this.buffs[i].GetBuffID() == buffID)
			{ // TODO: OnBuffExpires event
				this.buffs.RemoveAt(i);
				_manager.GetStatsScript().UpdateStats();
				return;
			}
			i++;
		}
	}

	public void RemoveBuffs()
	{
		this.buffs.Clear();
	}

	public void ReceiveBuff(CharacterManager inflictor, uint buffID, float duration = -1.0f)
	{
		if (DataTables.GetBuff(buffID) == null)
		{ // DEBUG
			return;
		}

		this.buffs.Add(new InflictedBuff(buffID, duration, inflictor));
		_manager.GetStatsScript().UpdateStats();
		_manager.GetEventScript().OnReceiveBuff(inflictor, buffID);
	}

	public void InflictBuff(CharacterManager target, uint buffID, float duration = -1.0f)
	{
		target.GetCombatScript().ReceiveBuff(_manager, buffID, duration);
	}

	public void ApplySpell(CharacterManager inflictor, Spell spell)
	{
		DamageInstance appliedSpell = new DamageInstance(inflictor, _manager, spell);
		this.combatLog.Add(appliedSpell);
	}

	public void Knockback(CharacterManager target, Vector3 dir, float speed, float duration)
	{ // Total distance travelled = dir.normalized * speed * duration <= Take fading into account now
		//InflictBuff(target, TODO, duration);
		target.GetMovementScript().SetMovementOverride(dir, speed, duration, true);
		target.GetEventScript().OnKnockback(_manager);
	}

	public bool CanUseSpell(uint spellID)
	{
		Spell spell = DataTables.GetSpell(spellID);

		if (spell == null)
		{
			return false;
		}

		if (_manager != GameData.activePlayer)
		{ // ;_;
			return true;
		}

		if (!_manager.GetCameraScript())
		{ // A monster... TEMPORARY :<
			return true;
		}

		if (!_manager.GetStatsScript().GetKnownSpells().Contains(spellID))
		{ // Unknown spell
			return false;
		}

		if (spell.GetCostType() == SpellCostType.MANA)
		{
			if (_manager.GetStatsScript().GetMana() < spell.GetCost(_manager))
			{
				return false;
			}
		}
		else if (spell.GetCostType() == SpellCostType.HEALTH)
		{
			if (_manager.GetStatsScript().GetHealth() < spell.GetCost(_manager))
			{
				return false;
			}
		}

		if (!spell.CastingCondition(_manager))
		{ // We don't meet some specific criteria
			return false;
		}

		return true;
	}

	public void UseSpell(uint spellID)
	{
		if (!CanUseSpell(spellID))
		{
			return;
		}

		Spell spell = DataTables.GetSpell(spellID);

		spell.Execute(_manager, _manager.GetInputScript().GetLookPosition(), _manager);

		if (spell.GetCostType() == SpellCostType.MANA)
		{
			_manager.GetStatsScript().LoseMana(spell.GetCost(_manager));
		}
		else if (spell.GetCostType() == SpellCostType.HEALTH)
		{ // TODO: We don't want to emit a pain event here
			_manager.GetStatsScript().LoseHealth(_manager, spell.GetCost(_manager));
		}
	}

	public void ResetCombatLog()
	{
		this.combatLog.Clear();
	}

	public void UpdateBuffs()
	{
		bool updated = false;
		for (int i = 0; i < buffs.Count; i++)
		{
			InflictedBuff buff = (InflictedBuff)buffs[i];
			if (GameData.gamePaused)
			{ // Don't lose the time spent with the game paused
				buff.AddToDuration(Time.deltaTime);
			}

			if (buff.GetTimeLeft() <= 0.0f)
			{ // TODO: OnBuffExpires event
				buffs.RemoveAt(i);
				i--;
				updated = true;
			}
		}

		if (updated)
		{
			_manager.GetStatsScript().UpdateStats();
		}
	}

	public List<InflictedBuff> GetBuffs()
	{
		return this.buffs;
	}

	public bool HasBuff(uint buffID)
	{
		foreach (InflictedBuff buff in this.buffs)
		{
			if (buff.GetBuffID() == buffID)
			{
				return true;
			}
		}
		return false;
	}

	public List<DamageInstance> GetCombatLog()
	{
		return this.combatLog;
	}
}
