using UnityEngine;
using System.Collections;

public abstract class SpellProj : Spell
{
	protected SpellProj(Metadata metadata, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f)
		: base(metadata, SpellType.PROJECTILE, costType, spellCost)
	{

	}

	protected abstract void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null);

	public override void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		this._Execute(inflictor, position, target);
	}
}