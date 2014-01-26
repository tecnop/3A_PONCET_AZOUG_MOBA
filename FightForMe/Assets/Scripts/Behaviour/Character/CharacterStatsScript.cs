using UnityEngine;
using System.Collections;

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
	float movementSpeed;	// Distance crossed by the player each second, in game units

	// Total stats obtained from items and skills; they affect other stats in various ways
	Stats stats;

	private CharacterInventoryScript _inventory;
	private CharacterCombatScript _combat;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_inventory = _manager.GetInventoryScript();
		_combat = _manager.GetCombatScript();

		this.UpdateStats();
	}

	public void UpdateStats()
	{
		if (this.health <= 0)
		{ // No need to do that now
			return;
		}

		Weapon weapon = _inventory.GetWeapon();
		ArrayList armorList = _inventory.GetAllArmor();

		// Initial values
		this.stats = Stats.Base;
		if (weapon != null)
		{
			this.damage = weapon.getDamage();
			this.attackRate = weapon.getAttackRate();
		}

		// Get all currently applied effects
		ArrayList buffList = new ArrayList(_combat.GetBuffs());
		ArrayList effects = new ArrayList();
		foreach (InflictedBuff buff in buffList)
		{
			effects.AddRange(buff.GetEffects());
		}

		foreach (Armor armor in armorList)
		{ // Tempted to make armor pieces apply the buff as they are equipped, buuuut... meh...
			foreach (uint effect in armor.getBuff().GetEffects())
			{
				effects.Add(DataTables.getEffect(effect));
			}
		}

		foreach (ArmorSet set in _inventory.GetCompletedSets())
		{ // Same deal here... will be the same for skills too
			foreach (uint effect in set.GetBuff().GetEffects())
			{
				effects.Add(DataTables.getEffect(effect));
			}
		}

		foreach (Effect effect in effects)
		{ // Have fun parsing it... really tempted to make everything public :|

		}

		// TODO: Get stats from items, armor sets, skills and buffs

		// TODO: Define stat conversion ratios

		// TODO: Properly handle max health and max mana changes
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
}
