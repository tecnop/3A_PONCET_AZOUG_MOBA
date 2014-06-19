using UnityEngine;
using System.Collections;

public enum SpellType
{
	TARGET,
	PROJECTILE,
	AREA
}

public enum SpellCostType
{
	NONE,
	MANA,
	HEALTH,
	PCTMANA,
	PCTHEALTH
}

public enum SpellCastingType
{
	DEFAULT,
	CONTINUOUS,
	CHANNELED
}

public abstract class Spell : WikiEntry
{
	private SpellType spellType;
	private SpellCostType costType;
	private float spellCost;
	private string castingSound;	// Name of the sound to play on cast

	public override WikiCategory category
	{
		get
		{
			return WikiCategory.SPELLS;
		}
	}

	// TODO:
	// - Flags for AIs
	// - Animations
	// - Casting speed, casting type

	protected Spell(Metadata metadata, SpellType spellType, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f, string castingSound = null)
		: base(metadata)
	{
		this.spellType = spellType;
		this.costType = costType;
		this.spellCost = spellCost;
		this.castingSound = castingSound;
	}

	public abstract void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null);

	public abstract bool CastingCondition(CharacterManager caster);

	public SpellType GetSpellType() { return this.spellType; }
	public SpellCostType GetCostType() { return this.costType; }
	public string GetCastingSound() { return this.castingSound; }

	public float GetCost(CharacterManager caster)
	{
		if (costType == SpellCostType.MANA || costType == SpellCostType.HEALTH)
			return spellCost;

		if (costType == SpellCostType.PCTMANA)
			return spellCost * caster.GetStatsScript().GetMaxMana();

		if (costType == SpellCostType.PCTHEALTH)
			return spellCost * caster.GetStatsScript().GetMaxHealth();

		return 0.0f;
	}
}
