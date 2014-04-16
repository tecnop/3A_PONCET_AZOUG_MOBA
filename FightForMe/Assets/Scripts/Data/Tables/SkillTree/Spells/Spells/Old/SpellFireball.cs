﻿using UnityEngine;
using System.Collections;

public class SpellFireball : SpellProj
{
	public SpellFireball()
		: base(new Metadata("Boule de feu", "Lance un projectile enflammé qui explose au contact avec un obstacle, brûlant tout ce qui se trouve aux alentours"), SpellCostType.MANA, 50.0f)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		/*Vector3 pos = inflictor.GetCharacterTransform().position;
		Quaternion dir = Quaternion.LookRotation(position - pos);
		CreateProjectile(inflictor, pos, dir, 2);*/
		inflictor.GetCombatScript().CreateProjectile(2);
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}