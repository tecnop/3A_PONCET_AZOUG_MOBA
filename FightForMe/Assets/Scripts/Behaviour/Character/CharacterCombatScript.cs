using UnityEngine;
using System.Collections;

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

	private ArrayList buffs;		// List of active buffs and debuffs (type: InflictedBuff)
	// TODO: Add a list of effects built from the list of buffs for quicker access? Would building it be more expensive than accessing each entry individually...?

	private ArrayList combatLog;	// List of previously applied damage instances, cleared upon respawn (type: DamageInstance)

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = this.transform;

		if (buffs == null)
		{
			buffs = new ArrayList();
		}

		if (combatLog == null)
		{
			combatLog = new ArrayList();
		}
	}

	public void Damage(CharacterManager target, float damage)
	{
		target.GetStatsScript().LoseHealth(_manager, damage);
	}

	public HitboxScript CreateAoE(Vector3 position, Quaternion angle, float radius, uint collisionSpellID)
	{
		GameObject sphere = (GameObject)Instantiate(damageSpherePrefab, position, angle);
		HitboxScript sphereScript = sphere.GetComponent<HitboxScript>();
		sphereScript.SetUp(_manager, radius, _manager.GetLayer(), collisionSpellID);
		return sphereScript;
	}

	public HitboxScript CreateAoE(float radius, uint collisionSpellID)
	{
		return CreateAoE(_transform.position, _transform.rotation, radius, collisionSpellID);
	}

	public ProjectileScript CreateProjectile(uint projectileID, Vector3 position, Quaternion angle, uint impactSpellOverride = 0)
	{
		GameObject proj = (GameObject)Instantiate(projectilePrefab, _transform.position, _transform.rotation);
		ProjectileScript projScript = proj.GetComponent<ProjectileScript>();
		projScript.SetUp(_manager, projectileID, impactSpellOverride);
		return projScript;
	}

	public ProjectileScript CreateProjectile(uint projectileID, uint impactSpellOverride = 0)
	{
		return CreateProjectile(projectileID, _transform.position, _transform.rotation, impactSpellOverride);
	}

	public void ReceiveBuff(CharacterManager inflictor, uint buffID, float duration)
	{
		if (DataTables.GetBuff(buffID) == null)
		{ // DEBUG
			return;
		}

		this.buffs.Add(new InflictedBuff(buffID, duration, inflictor));
		_manager.GetStatsScript().UpdateStats();
		_manager.GetEventScript().OnReceiveBuff(inflictor, buffID);
	}

	public void InflictBuff(CharacterManager target, uint buffID, float duration)
	{
		target.GetCombatScript().ReceiveBuff(_manager, buffID, duration);
	}

	public void ApplySpell(CharacterManager inflictor, Spell spell)
	{
		DamageInstance appliedSpell = new DamageInstance(inflictor, _manager, spell);
		this.combatLog.Add(appliedSpell);
	}

	public bool CanUseSpell(uint spellID)
	{
		Spell spell = DataTables.GetSpell(spellID);

		if (spell == null)
		{
			return false;
		}

		if (!GameData.isServer)
		{ // ;_;
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
		{ // Don't really want to use that function actually, change it maybe?
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
			{
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

	public ArrayList GetBuffs()
	{
		return this.buffs;
	}

	public ArrayList GetCombatLog()
	{
		return this.combatLog;
	}
}
