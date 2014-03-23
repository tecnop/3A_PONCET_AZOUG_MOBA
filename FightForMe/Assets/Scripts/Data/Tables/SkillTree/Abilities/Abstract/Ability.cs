using UnityEngine;
using System.Collections;

public enum AbilityType
{
	TARGET,
	PROJECTILE,
	AREA
}

public abstract class Ability : WikiEntry
{
	AbilityType abilityType;

	// TODO: Health cost, mana cost, pct health cost, pct mana cost

	public Ability(Metadata metadata, AbilityType abilityType)
		: base(metadata)
	{

	}

	public abstract void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null);
}
