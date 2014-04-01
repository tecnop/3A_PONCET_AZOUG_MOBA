using UnityEngine;
using System.Collections;

public class SpellKnockback : SpellTarget
{
	public SpellKnockback()
		: base(new Metadata("Expulsion", "Repousse la cible sur une certaine distance"), SpellCostType.NONE)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{ // So how do we do this?
		//inflictor.GetCombatScript().InflictBuff(target, 3, 5.0f);
	}

	public override float GetCost(CharacterManager caster)
	{
		return 0;
	}
}
