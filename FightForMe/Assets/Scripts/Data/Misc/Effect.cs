using UnityEngine;
using System.Collections;

public enum MiscEffect
{
	NONE,
	HEAVY_WEAPONS
}

public class Effect
{
	private string description;		// Descriptive text of the effect
	private bool isPositive;		// If true, the effect is an improvement, otherwise it is negative

	// Split that into child classes maybe? Also tempted to make everything public... *sigh*
	private float flatHP, pctHP;
	private float flatMP, pctMP;
	private float flatHPRegen, pctHPRegen;
	private float flatMPRegen, pctMPRegen;
	private float flatMS, pctMS;

	private float bonusDamage, bonusAtkSpd;		// Percentages

	private Stats stats;

	private uint unlockedAbility;		// ID of the active skill we unlock if any

	private MiscEffect misc;		// Any other boolean effect that comes with this effect
	private float miscParm;			// Parameter for the misc value (if applicable)

	public Effect(string description,
		bool isPositive,
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
		Stats stats = null,
		uint unlockedAbility = 0,
		MiscEffect misc = MiscEffect.NONE,
		float miscParm = 0.0f)
	{
		this.description = description;
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
	}

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
	public Stats GetStats() { return this.stats; }
	public uint GetUnlockedAbility() { return this.unlockedAbility; }
	public MiscEffect GetMiscEffect() { return this.misc; }
	public float GetMiscParm() { return this.miscParm; }
}