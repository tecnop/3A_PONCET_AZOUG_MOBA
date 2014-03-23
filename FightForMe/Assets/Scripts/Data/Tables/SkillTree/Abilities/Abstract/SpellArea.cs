using UnityEngine;
using System.Collections;

public abstract class SpellArea : Ability
{
	public SpellArea(Metadata metadata)
		: base(metadata, AbilityType.AREA)
	{

	}
}