using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * CharacterStatsScript.cs
 * 
 * Calculates and stores the character's various stats
 * 
 */

public class CharacterStatsScript : MonoBehaviour
{
	// Stat conversion constants
	private const float strToMaxHealth = 10.0f;
	private const float strToHealthRegen = 0.1f;
	private const float agiToDamage = 0.5f;
	private const float intToMaxMana = 7.5f;
	private const float intToManaRegen = 0.2f;

	[SerializeField]
	private NetworkView _networkView;

	private CharacterManager _manager;

	// Ressources
	private float health;
	private float mana;

	// Stats
	private float maxHealth;
	private float healthRegen;		// In HP per second

	private float maxMana;
	private float manaRegen;		// In MP per second

	private float damage;			// Damage per swing of the equipped weapon
	private float projDamage;		// Damage of each projectile shot by the equipped weapon (TODO)
	private float attackRate;		// Swings per second of the equipped weapon
	private float movementSpeed;	// Distance crossed by the player each second, in 0.01 of a game unit

	// Total stats obtained from items and skills; they affect other stats in various ways
	private Stats stats;

	private List<uint> knownSpells;	// List of all known spells

	private uint specialEffects;	// Special effect flags

	private CharacterInventoryScript _inventory;
	private CharacterCombatScript _combat;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_inventory = _manager.GetInventoryScript();
		_combat = _manager.GetCombatScript();

		knownSpells = new List<uint>();

		this.UpdateStats(true);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{ // Do we want this? I sure as hell don't
		//if (stream.isWriting)
		if (GameData.isServer)
		{
			stream.Serialize(ref this.health);
			stream.Serialize(ref this.mana);
		}
		else
		{
			stream.Serialize(ref this.health);
			stream.Serialize(ref this.mana);
		}
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
		List<Armor> armorList = _inventory.GetAllArmor();

		// Initial values
		this.maxHealth = this.maxMana = 0.0f;
		this.healthRegen = this.manaRegen = 0.0f;
		if (weapon != null)
		{
			this.damage = weapon.GetDamage();
			this.attackRate = weapon.GetAttackRate();
			Projectile proj = weapon.GetProjectile();
			if (proj != null)
			{
				this.projDamage = proj.GetDamage();
			}
		}
		else
		{ // Use your fists! (Damage is only gained from agility)
			this.damage = 0.0f;
			this.projDamage = 0.0f;
			this.attackRate = 1.0f;
		}
		this.movementSpeed = 350.0f;
		this.stats = Stats.Base;		// Do we need that anymore? Why not define it here?
		this.knownSpells.Clear();
		this.specialEffects = 0;

		// Get all currently applied effects
		List<InflictedBuff> buffList = new List<InflictedBuff>(_combat.GetBuffs());
		List<Effect> effects = new List<Effect>();
		foreach (InflictedBuff buff in buffList)
		{
			effects.AddRange(buff.GetEffects());
		}

		foreach (Armor armor in armorList)
		{ // Tempted to make armor pieces apply the buff as they are equipped, buuuut... meh...
			Buff buff = armor.GetBuff();
			if (buff != null)
			{
				foreach (uint effect in buff.GetEffects())
				{
					effects.Add(DataTables.GetEffect(effect));
				}
			}

			this.stats += armor.GetStats(); // Armors have this little extra thing
		}

		foreach (ArmorSet set in _inventory.GetCompletedSets())
		{ // Same deal here... will be the same for skills too. Consider doing that for beta maybe?
			Buff buff = set.GetBuff();
			if (buff != null)
			{
				foreach (uint effect in buff.GetEffects())
				{
					effects.Add(DataTables.GetEffect(effect));
				}
			}
		}

		if (_manager.GetCameraScript())
		{ // THIS IS A TERRIBLE WAY TO CHECK IF WE'RE A PLAYER
			List<Skill> unlockedSkills = ((PlayerMiscDataScript)_manager.GetMiscDataScript()).GetUnlockedSkills();
			foreach (Skill skill in unlockedSkills)
			{
				Effect effect = skill.GetEffect();
				if (effect != null)
				{
					effects.Add(effect);
				}
			}
		}

		float maxHealthPctChange = 1.0f;
		float maxManaPctChange = 1.0f;
		float healthRegenPct = 0.0f;
		float manaRegenPct = 0.0f;
		float moveSpeedPctChange = 1.0f;
		float damagePctChange = 1.0f;
		float projDamagePctChange = 1.0f;

		foreach (Effect effect in effects)
		{ // I had a lot of fun writing this particular bit
			this.maxHealth += effect.GetFlatHP();
			maxHealthPctChange += effect.GetPctHP();
			this.maxMana += effect.GetFlatMP();
			maxManaPctChange += effect.GetPctMP();
			this.healthRegen += effect.GetFlatHPRegen();
			healthRegenPct += effect.GetPctHPRegen();
			this.manaRegen += effect.GetFlatMPRegen();
			manaRegenPct += effect.GetPctMPRegen();
			this.movementSpeed += effect.GetFlatMS();
			moveSpeedPctChange += effect.GetPctMS();
			damagePctChange += effect.GetBonusDamage();
			projDamagePctChange += effect.GetBonusProjDamage();
			this.attackRate += effect.GetBonusAtkSpd();
			this.stats += effect.GetStats();

			if (effect.GetUnlockedAbility() != 0)
			{
				this.knownSpells.Add(effect.GetUnlockedAbility());
			}

			MiscEffect miscEffect = effect.GetMiscEffect();
			if (miscEffect != MiscEffect.NONE)
			{
				this.specialEffects += (uint)(1 << (int)(miscEffect-1));
			}
		}

		// Convert stats into... er... more stats.
		this.maxHealth += strToMaxHealth * this.stats.GetStrength();
		this.healthRegen += strToHealthRegen * this.stats.GetStrength();
		this.damage += agiToDamage * this.stats.GetAgility();
		this.maxMana += intToMaxMana * this.stats.GetIntelligence();
		this.manaRegen += intToManaRegen * this.stats.GetIntelligence();

		// Apply percentage-based stat modifiers
		this.maxHealth *= maxHealthPctChange;
		this.maxMana *= maxManaPctChange;
		this.healthRegen += this.maxHealth * healthRegenPct;
		this.manaRegen += this.maxMana * manaRegenPct;
		this.movementSpeed *= moveSpeedPctChange;
		this.damage *= damagePctChange;
		this.projDamage *= projDamagePctChange;

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
			_manager.GetEventScript().OnDeath(_manager); // Count it as a suicide for now
			return;
		}

		if (this.health > this.maxHealth)
		{
			this.health = this.maxHealth;
		}

		this.mana = Mathf.Clamp(this.mana + this.manaRegen * Time.deltaTime, 0.0f, this.maxMana);
	}

	public void Revive()
	{
		if (health > 0)
		{ // We're not dead...
			return;
		}

		health = maxHealth;
		mana = maxMana;

		_manager.GetCharacterAnimator().SetBool("isDead", false);
		_manager.GetCombatScript().ResetCombatLog();
	}

	public void GainHealth(float amount)
	{
		if (health <= 0)
		{ // Nope
			return;
		}

		health += amount;
		if (health > maxHealth)
		{
			health = maxHealth;
		}
	}

	public void GainMana(float amount)
	{
		if (health <= 0)
		{ // Nope
			return;
		}

		mana += amount;
		if (mana > maxMana)
		{
			mana = maxMana;
		}
	}

	public void LoseHealth(CharacterManager inflictor, float amount)
	{
		if (health <= 0)
		{ // Nope
			return;
		}

		health -= amount;

		if (health <= 0)
		{
			health = 0;
			_manager.GetEventScript().OnDeath(inflictor);
		}
		else
		{
			_manager.GetEventScript().OnPain(inflictor, amount);
		}
	}

	public void LoseMana(float amount)
	{
		if (health <= 0)
		{ // Nope
			return;
		}

		mana -= amount;
		if (mana <= 0)
		{
			mana = 0;
		}
	}

	public float GetHealth() { return this.health; }
	public float GetMana() { return this.mana; }
	public float GetMaxHealth() { return this.maxHealth; }
	public float GetHealthRegen() { return this.healthRegen; }
	public float GetMaxMana() { return this.maxMana; }
	public float GetManaRegen() { return this.manaRegen; }
	public float GetDamage() { return this.damage; }
	public float GetAttackRate() { return this.attackRate; }
	public float GetProjDamage() { return this.projDamage; }
	public float GetMovementSpeed() { return this.movementSpeed; }
	public Stats GetStats() { return this.stats; }
	public List<uint> GetKnownSpells() { return this.knownSpells; }
	public uint GetSpecialEffects() { return this.specialEffects; }
}
