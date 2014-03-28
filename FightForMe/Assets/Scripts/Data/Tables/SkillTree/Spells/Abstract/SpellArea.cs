using UnityEngine;
using System.Collections;

public abstract class SpellArea : Spell
{
	protected SpellArea(Metadata metadata, SpellCostType costType)
		: base(metadata, SpellType.AREA, costType)
	{

	}

	protected abstract void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null);

	public override void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		this._Execute(inflictor, position, target);
	}
}