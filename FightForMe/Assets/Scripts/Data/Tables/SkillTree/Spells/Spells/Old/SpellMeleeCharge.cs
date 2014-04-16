﻿using UnityEngine;
using System.Collections;

public class SpellMeleeCharge : SpellTarget
{
	public SpellMeleeCharge()
		: base(new Metadata("Attaque chargée", "Prépare le prochain coup pour infliger des dégâts supplémentaires"), SpellCostType.MANA, 0.0f)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		inflictor.GetCombatScript().InflictBuff(target, 6, 15.0f);
	}

	public override bool CastingCondition(CharacterManager caster)
	{ // TODO: Melee only? Also must not be active
		return true;
	}
}