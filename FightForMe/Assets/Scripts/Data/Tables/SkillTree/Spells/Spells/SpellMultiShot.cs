using UnityEngine;
using System.Collections;

public class SpellMultiShot : SpellProj
{
	public SpellMultiShot()
		: base(new Metadata("Tir multiple", "Tire plusieurs projectiles à la fois"), SpellCostType.MANA)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		
	}

	public override float GetCost(CharacterManager caster)
	{
		return 0;
	}

	public override bool CastingCondition(CharacterManager caster)
	{ // TODO: Bow only
		return true;
	}
}