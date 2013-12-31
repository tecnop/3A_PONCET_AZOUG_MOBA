using UnityEngine;
using System.Collections;

public class CharacterStatsScript : MonoBehaviour
{
	[SerializeField]
	CharacterManager _manager;

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

	public void UpdateStats()
	{

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

	public void loseHealth(float amount)
	{
		health -= amount;

		if (health <= 0)
		{
			health = 0;
			_manager.GetEventScript().OnDeath(null);
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
