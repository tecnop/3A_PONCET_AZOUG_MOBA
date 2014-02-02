using UnityEngine;
using System.Collections;

/*
 * CharacterStatsScript.cs
 * 
 * Calculates and stores the character's various stats
 * 
 */

public class CharacterStatsScript : MonoBehaviour
{
	private CharacterManager _manager;

	// Ressources
	float health;
	float mana;

	// Stats
	float maxHealth;
	float healthRegen;		// In HP per second

	float maxMana;
	float manaRegen;		// In MP per second

	float damage;			// Damage per swing of the equipped weapon
	float attackRate;		// Swings per second of the equipped weapon
	float movementSpeed;	// Distance crossed by the player each second, in 0.01 of a game unit

	// Total stats obtained from items and skills; they affect other stats in various ways
	Stats stats;

	private CharacterInventoryScript _inventory;
	private CharacterCombatScript _combat;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_inventory = _manager.GetInventoryScript();
		_combat = _manager.GetCombatScript();

		this.UpdateStats(true);
	}

	public void UpdateStats(bool firstTime = false)
	{
		if (!firstTime && this.health <= 0)
		{ // No need to do that now
			return;
		}

		float savedMaxHealth = this.maxHealth;
		float savedMaxMana = this.maxMana;

		Weapon weapon = _inventory.GetWeapon();
		ArrayList armorList = _inventory.GetAllArmor();

		// Initial values
		this.maxHealth = this.maxMana = 0.0f;
		this.healthRegen = this.manaRegen = 0.0f;
		if (weapon != null)
		{
			this.damage = weapon.getDamage();
			this.attackRate = weapon.getAttackRate();
		}
		else
		{
			this.damage = 0.0f;
			this.attackRate = 1.0f;
		}
		this.movementSpeed = 350.0f;
		this.stats = Stats.Base;		// Do we need that anymore? Why not define it here?

		// Get all currently applied effects
		ArrayList buffList = new ArrayList(_combat.GetBuffs());
		ArrayList effects = new ArrayList();
		foreach (InflictedBuff buff in buffList)
		{
			effects.AddRange(buff.GetEffects());
		}

		foreach (Armor armor in armorList)
		{ // Tempted to make armor pieces apply the buff as they are equipped, buuuut... meh...
			Buff buff = armor.getBuff();
			if (buff != null)
			{
				foreach (uint effect in buff.GetEffects())
				{
					effects.Add(DataTables.getEffect(effect));
				}
			}
		}

		foreach (ArmorSet set in _inventory.GetCompletedSets())
		{ // Same deal here... will be the same for skills too. Consider doing that for beta maybe?
			Buff buff = set.GetBuff();
			if (buff != null)
			{
				foreach (uint effect in buff.GetEffects())
				{
					effects.Add(DataTables.getEffect(effect));
				}
			}
		}

		// TODO: Buffs from skills

		float maxHealthPctChange = 1.0f;
		float maxManaPctChange = 1.0f;
		float healthRegenPct = 0.0f;
		float manaRegenPct = 0.0f;
		float moveSpeedPctChange = 1.0f;
		float damagePctChange = 1.0f;

		foreach (Effect effect in effects)
		{ // Have fun parsing it... really tempted to make everything public :|
			this.maxHealth += effect.GetFlatHP();
			maxHealthPctChange *= effect.GetPctHP();
			this.maxMana += effect.GetFlatMP();
			maxManaPctChange *= effect.GetPctMP();
			this.healthRegen += effect.GetFlatHPRegen();
			healthRegenPct += effect.GetPctHPRegen();
			this.manaRegen += effect.GetFlatMPRegen();
			manaRegenPct += effect.GetPctMPRegen();
			this.movementSpeed += effect.GetFlatMS();
			moveSpeedPctChange += effect.GetPctMS();
			damagePctChange += effect.GetBonusDamage();
			this.attackRate += effect.GetBonusAtkSpd();
			this.stats += effect.GetStats();
			// TODO: Unlock effect.GetUnlockedAbility() when skill tree is implemented
			// Also use effect.GetMiscEffect() somewhere? Does it belong here?
		}

		// Convert stats into... er... stats.
		// I wanted to #define those ratios but apparently that doesn't work in C#? No internet atm :(
		this.maxHealth		+= 10.0f	* this.stats.GetStrength();
		this.healthRegen	+=  0.1f	* this.stats.GetStrength();
		this.damage			+=  0.5f	* this.stats.GetAgility();
		this.maxMana		+=  7.5f	* this.stats.GetIntelligence();
		this.manaRegen		+=  0.2f	* this.stats.GetIntelligence();

		// Apply percentage-based stat modifiers
		this.maxHealth *= maxHealthPctChange;
		this.maxMana *= maxManaPctChange;
		this.healthRegen += this.maxHealth * healthRegenPct;
		this.manaRegen += this.maxMana * manaRegenPct;
		this.movementSpeed *= moveSpeedPctChange;
		this.damage *= damagePctChange;

		if (this.maxHealth < 1) this.maxHealth = 1;
		if (this.maxMana < 1) this.maxMana = 1;		// This might be possible later, but not yet

		if (firstTime)
		{
			this.health = this.maxHealth;
			this.mana = this.maxMana;
		}
		else
		{ // If our max health and/or max mana changed, adapt our health and mana accordingly so that our current percentage remains the same
			float healthChange = this.maxHealth / savedMaxHealth;
			float manaChange = this.maxMana / savedMaxMana;

			this.health *= healthChange;
			this.mana *= manaChange;
		}
	}

	public void ApplyRegen()
	{
		if (this.health <= 0)
		{ // No regeneration when dead please
			return;
		}

		this.health += this.healthRegen * Time.deltaTime;

		if (this.health <= 0)
		{ // Died from damage over time... need to find the killer somehow. TODO: Combat script should keep track of that
			_manager.GetEventScript().OnDeath(_manager);
			return;
		}

		if (this.health > this.maxHealth)
		{
			this.health = this.maxHealth;
		}

		this.mana = Mathf.Clamp(this.mana + this.manaRegen * Time.deltaTime, 0.0f, this.maxMana);
	}

	public void gainHealth(float amount)
	{
		health += amount;
		if (health > maxHealth)
		{
			health = maxHealth;
		}
	}

	public void gainMana(float amount)
	{
		mana += amount;
		if (mana > maxMana)
		{
			mana = maxMana;
		}
	}

	public void loseHealth(CharacterManager inflictor, float amount)
	{
		health -= amount;

		Debug.Log(_manager.name + " took " + amount + " damage");

		if (health <= 0)
		{
			health = 0;
			_manager.GetEventScript().OnDeath(inflictor);
		}
		else
		{
			_manager.GetEventScript().OnPain(amount);
		}
	}

	public void loseMana(float amount)
	{
		mana -= amount;
		if (mana <= 0)
		{
			mana = 0;
		}
	}

	public float GetMovementSpeed()
	{
		return this.movementSpeed;
	}
}
