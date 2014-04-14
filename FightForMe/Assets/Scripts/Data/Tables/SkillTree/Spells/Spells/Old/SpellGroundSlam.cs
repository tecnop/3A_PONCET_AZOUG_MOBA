using UnityEngine;
using System.Collections;

public class SpellGroundSlam : SpellTarget
{
	public SpellGroundSlam()
		: base(new Metadata("Onde de choc", "Frappe le sol et paralyse les ennemis à portée"), SpellCostType.MANA, 0.0f)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}