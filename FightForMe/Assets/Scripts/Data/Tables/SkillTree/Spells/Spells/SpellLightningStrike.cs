﻿using UnityEngine;
using System.Collections;

public class SpellLightningStrike : SpellArea
{
	public SpellLightningStrike()
		: base(new Metadata("Eclair", "Frappe la zone ciblée avec un puissant éclair"), SpellCostType.MANA)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		inflictor.GetCombatScript().CreateAoE(position, Quaternion.identity, 8.0f, 0); // electrical discharge goes here
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