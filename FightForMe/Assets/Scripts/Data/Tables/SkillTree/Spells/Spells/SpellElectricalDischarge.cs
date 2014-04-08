using UnityEngine;
using System.Collections;

public class SpellElectricalDischarge : SpellTarget
{
	public SpellElectricalDischarge()
		: base(new Metadata("Décharge électrique", "Inflige des dégâts et paralyse temporairement cible"))
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		inflictor.GetCombatScript().Damage(target, 75.0f);
		//inflictor.GetCombatScript().InflictBuff(target, 3, 5.0f);
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