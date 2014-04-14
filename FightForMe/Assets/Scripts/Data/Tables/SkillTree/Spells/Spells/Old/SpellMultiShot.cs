using UnityEngine;
using System.Collections;

public class SpellMultiShot : SpellProj
{
	public SpellMultiShot()
		: base(new Metadata("Tir multiple", "Tire plusieurs projectiles à la fois"), SpellCostType.MANA, 0.0f)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		
	}

	public override bool CastingCondition(CharacterManager caster)
	{ // TODO: Bow only
		return true;
	}
}