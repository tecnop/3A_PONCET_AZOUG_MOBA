using UnityEngine;
using System.Collections;

public enum SpellType
{
	TARGET,
	PROJECTILE,
	AREA
}

public abstract class Spell : WikiEntry
{
	SpellType abilityType;

	// TODO:
	// - Health cost, mana cost, pct health cost, pct mana cost
	// - Flags for AIs
	// - Animations
	// - Casting speed, casting type

	public Spell(Metadata metadata, SpellType abilityType)
		: base(metadata)
	{

	}

	public abstract void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null);
}
