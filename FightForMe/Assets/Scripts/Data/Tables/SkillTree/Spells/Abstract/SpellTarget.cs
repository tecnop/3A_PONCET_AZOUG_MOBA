using UnityEngine;
using System.Collections;

public abstract class SpellTarget : Spell
{
	public SpellTarget(Metadata metadata)
		: base(metadata, SpellType.TARGET)
	{

	}
}