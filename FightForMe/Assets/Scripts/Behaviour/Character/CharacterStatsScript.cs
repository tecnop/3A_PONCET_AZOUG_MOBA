using UnityEngine;
using System.Collections;

public class CharacterStatsScript : MonoBehaviour
{
	[SerializeField]
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

	// Total stats obtained from items and skills; they are used to recalculate every other stat
	Stats stats;

	public void UpdateStats()
	{
		this.stats = Stats.Base;

		// TODO: Get stats from items, skills and buffs

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
