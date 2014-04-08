using UnityEngine;
using System.Collections;

public class SpellRage : SpellTarget
{
	public SpellRage()
		: base(new Metadata("Rage", "Augmente la puissance mais inflige des dégâts sur la durée"), SpellCostType.NONE)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		inflictor.GetCombatScript().InflictBuff(target, 7, 5.0f);
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