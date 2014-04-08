using UnityEngine;
using System.Collections;

public class SpellGrenade : SpellProj
{
	public SpellGrenade()
		: base(new Metadata("Grenade", "Ça va pêter!"), SpellCostType.MANA)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		inflictor.GetCombatScript().CreateProjectile(5).ThrowAt(position);
	}

	public override float GetCost(CharacterManager caster)
	{
		//return 150;
		return 0;
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}
