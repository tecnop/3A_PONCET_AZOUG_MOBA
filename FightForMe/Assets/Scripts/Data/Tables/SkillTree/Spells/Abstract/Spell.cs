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
	HEALTH
}

public enum SpellCastingType
{
	DEFAULT,
	CONTINUOUS,
	CHANNELED
}

public abstract class Spell : WikiEntry
{
	SpellType spellType;
	SpellCostType costType;

	// TODO:
	// - Health cost, mana cost, pct health cost, pct mana cost
	// - Flags for AIs
	// - Animations
	// - Casting speed, casting type

	protected Spell(Metadata metadata, SpellType spellType, SpellCostType costType)
		: base(metadata)
	{
		this.spellType = spellType;
		this.costType = costType;
	}

	public abstract void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null);

	public abstract float GetCost(CharacterManager caster);

	public SpellType GetSpellType() { return this.spellType; }
	public SpellCostType GetCostType() { return this.costType; }
}
