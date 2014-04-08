using UnityEngine;
using System.Collections;

public class SpellMeleeHit : SpellTarget
{
	public SpellMeleeHit()
		: base(new Metadata("Coup au corps à corps", "Inflige un coup avec l'arme au corps à corps équipée"), SpellCostType.NONE)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		if (target != null)
		{
			//Vector3 dir = (target.GetCharacterTransform().position - position).normalized;
			target.GetCombatScript().ApplySpell(inflictor, this);
			inflictor.GetCombatScript().Damage(target, inflictor.GetStatsScript().GetDamage());
		}
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
