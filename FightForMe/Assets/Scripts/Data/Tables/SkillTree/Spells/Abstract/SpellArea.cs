using UnityEngine;
using System.Collections;

public abstract class SpellArea : Spell
{
	public SpellArea(Metadata metadata)
		: base(metadata, SpellType.AREA)
	{

	}
}