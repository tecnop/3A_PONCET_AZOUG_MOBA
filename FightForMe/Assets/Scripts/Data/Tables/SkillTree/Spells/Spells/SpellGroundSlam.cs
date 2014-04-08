using UnityEngine;
using System.Collections;

public class SpellGroundSlam : SpellTarget
{
	public SpellGroundSlam()
		: base(new Metadata("Onde de choc", "Frappe le sol et paralyse les ennemis à portée"), SpellCostType.MANA)
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
	{
		return true;
	}
}