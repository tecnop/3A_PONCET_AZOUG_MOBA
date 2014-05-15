using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MiscEffect
{ // Special effect are stored as flags, so try to have less than 32 of them
	NONE,				// None
	HEAVY_WEAPONS,		// Character may use heavy weapons and armor
	LOSS_OF_CONTROL,	// Character cannot move anymore and is made non-kinematic (also implies a movement override)
	CARRYING_TROPHY,	// Character is carrying the trophy
	INVULNERABLE		// Self-explanatory
}

public class Effect
{
	private string description;		// Descriptive text of the effect (used to create a buff's description)
	private bool isPositive;		// If true, the effect is an improvement, otherwise it is negative

	// Split that into child classes maybe?
	private float flatHP, pctHP;
	private float flatMP, pctMP;
	private float flatHPRegen, pctHPRegen;
	private float flatMPRegen, pctMPRegen;
	private float flatMS, pctMS;

	private float bonusDamage, bonusAtkSpd, bonusProjDamage;	// Percentages

	private Stats stats;

	private uint unlockedAbility;		// ID of the active skill we unlock if any

	private MiscEffect misc;		// Any other boolean effect that comes with this effect
	private float miscParm;			// Parameter for the misc value (if applicable)

	public Effect(bool isPositive,
		string description = null,
		float flatHP = 0.0f,
		float pctHP = 0.0f,
		float flatMP = 0.0f,
		float pctMP = 0.0f,
		float flatHPRegen = 0.0f,
		float pctHPRegen = 0.0f,
		float flatMPRegen = 0.0f,
		float pctMPRegen = 0.0f,
		float flatMS = 0.0f,
		float pctMS = 0.0f,
		float bonusDamage = 0.0f,
		float bonusAtkSpd = 0.0f,
		float bonusProjDamage = 0.0f,
		Stats stats = null,
		uint unlockedAbility = 0,
		MiscEffect misc = MiscEffect.NONE,
		float miscParm = 0.0f)
	{
		this.isPositive = isPositive;

		this.flatHP = flatHP;
		this.pctHP = pctHP;
		this.flatMP = flatMP;
		this.pctMP = pctMP;
		this.flatHPRegen = flatHPRegen;
		this.pctHPRegen = pctHPRegen;
		this.flatMPRegen = flatMPRegen;
		this.pctMPRegen = pctMPRegen;
		this.flatMS = flatMS;
		this.pctMS = pctMS;
		this.bonusDamage = bonusDamage;
		this.bonusAtkSpd = bonusAtkSpd;
		this.bonusProjDamage = bonusProjDamage;
		if (stats == null)
		{
			this.stats = new Stats();
		}
		else
		{
			this.stats = stats;
		}
		this.unlockedAbility = unlockedAbility;
		this.misc = misc;
		this.miscParm = miscParm;

		// This must stay at the end!
		if (description != null)
		{
			this.description = description;
		}
		else
		{
			this.description = BuildDescription();
		}
	}

	private string BuildDescription()
	{
		List<string> list = new List<string>();

		if (this.flatHP != 0.0f)
		{
			string temp = this.flatHP < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " les points de vie de l'utilisateur de " + Mathf.Abs(this.flatHP));
		}
		if (this.pctHP != 0.0f)
		{
			string temp = this.pctHP < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " les points de vie de l'utilisateur de <maxhealth " + Mathf.Abs(this.pctHP)+ " />");
		}
		if (this.flatMP != 0.0f)
		{
			string temp = this.flatMP < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " les points de mana de l'utilisateur de " + Mathf.Abs(this.flatMP));
		}
		if (this.pctMP != 0.0f)
		{
			string temp = this.pctMP < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " les points de mana de l'utilisateur de <maxmana " + Mathf.Abs(this.pctMP) + " />");
		}
		if (this.flatHPRegen != 0.0f)
		{
			string temp = this.flatHPRegen < 0 ? "Fait perdre" : "Rend";
			list.Add(temp + " à l'utilisateur " + Mathf.Abs(this.flatHPRegen) + " points de vie par seconde");
		}
		if (this.pctHPRegen != 0.0f)
		{
			string temp = this.pctHPRegen < 0 ? "Fait perdre" : "Rend";
			list.Add(temp + " à l'utilisateur <maxhealth " + Mathf.Abs(this.pctHPRegen)+ "> par seconde");
		}
		if (this.flatMPRegen != 0.0f)
		{
			string temp = this.flatMPRegen < 0 ? "Fait perdre" : "Rend";
			list.Add(temp + " à l'utilisateur " + Mathf.Abs(this.flatMPRegen) + " points de mana par seconde");
		}
		if (this.pctMPRegen != 0.0f)
		{
			string temp = this.pctMPRegen < 0 ? "Fait perdre" : "Rend";
			list.Add(temp + " à l'utilisateur <maxmana " + Mathf.Abs(this.pctMPRegen) + "> par seconde");
		}
		if (this.flatMS != 0.0f)
		{
			string temp = this.flatMS < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " la vitesse de déplacement de l'utilisateur de " + Mathf.Abs(this.flatMS));
		}
		if (this.pctMS != 0.0f)
		{
			string temp = this.pctMS < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " la vitesse de déplacement de l'utilisateur de <movespeed " + Mathf.Abs(this.pctMS) + " />");
		}
		if (this.bonusDamage != 0.0f)
		{
			string temp = this.bonusDamage < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " les dégâts de l'utilisateur de <dmg " + Mathf.Abs(this.bonusDamage) + " />");
		}
		if (this.bonusAtkSpd != 0.0f)
		{ // FIXME: I don't remember how this one works
			string temp = this.bonusAtkSpd < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " la vitesse d'attaque de l'utilisateur de " + Mathf.Abs(this.bonusAtkSpd) * 100 + "%");
		}
		if (this.bonusProjDamage != 0.0f)
		{
			string temp = this.bonusProjDamage < 0 ? "Réduit" : "Augmente";
			list.Add(temp + " les dégâts des projectiles de l'utilisateur de <projdmg " + Mathf.Abs(this.bonusProjDamage) + " />");
		}
		if (this.stats != null)
		{
			if (this.stats.GetStrength() != 0)
			{
				string temp = this.stats.GetStrength() < 0 ? "Réduit" : "Augmente";
				list.Add(temp + " l'endurance de l'utilisateur de " + Mathf.Abs(this.stats.GetStrength()));
			}
			if (this.stats.GetAgility() != 0)
			{
				string temp = this.stats.GetAgility() < 0 ? "Réduit" : "Augmente";
				list.Add(temp + " la puissance de l'utilisateur de " + Mathf.Abs(this.stats.GetAgility()));
			}
			if (this.stats.GetIntelligence() != 0)
			{
				string temp = this.stats.GetIntelligence() < 0 ? "Réduit" : "Augmente";
				list.Add(temp + " l'intelligence de l'utilisateur de " + Mathf.Abs(this.stats.GetIntelligence()));
			}
		}
		if (this.unlockedAbility != 0)
		{
			list.Add("Débloque le sort: " + DataTables.GetSpell(this.unlockedAbility).GetName());
		}
		if (this.misc != MiscEffect.NONE)
		{ // TODO
			list.Add(this.misc.ToString());
		}

		return string.Join("\n", list.ToArray());
	}

	public string GetDescription() { return this.description; }
	public bool IsPositive() { return this.isPositive; }

	// Here we go...
	public float GetFlatHP() { return this.flatHP; }
	public float GetPctHP() { return this.pctHP; }
	public float GetFlatMP() { return this.flatMP; }
	public float GetPctMP() { return this.pctMP; }
	public float GetFlatHPRegen() { return this.flatHPRegen; }
	public float GetPctHPRegen() { return this.pctHPRegen; }
	public float GetFlatMPRegen() { return this.flatMPRegen; }
	public float GetPctMPRegen() { return this.pctMPRegen; }
	public float GetFlatMS() { return this.flatMS; }
	public float GetPctMS() { return this.pctMS; }
	public float GetBonusDamage() { return this.bonusDamage; }
	public float GetBonusAtkSpd() { return this.bonusAtkSpd; }
	public float GetBonusProjDamage() { return this.bonusProjDamage; }
	public Stats GetStats() { return this.stats; }
	public uint GetUnlockedAbility() { return this.unlockedAbility; }
	public MiscEffect GetMiscEffect() { return this.misc; }
	public float GetMiscParm() { return this.miscParm; }
}