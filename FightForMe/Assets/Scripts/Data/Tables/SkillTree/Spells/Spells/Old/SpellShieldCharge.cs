using UnityEngine;
using System.Collections;

public class SpellShieldCharge : SpellTarget
{
	public SpellShieldCharge()
		: base(new Metadata("Charge au bouclier", "Propulse l'utilisateur vers l'avant, infligeant des dégâts et repoussant tout ennemi sur son passage"), SpellCostType.MANA, 0.0f)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		//inflictor.GetCombatScript().InflictBuff(target, 3, 5.0f);
	}

	public override bool CastingCondition(CharacterManager caster)
	{ // TODO: Shield
		return true;
	}
}