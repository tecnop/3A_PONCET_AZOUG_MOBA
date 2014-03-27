using UnityEngine;
using System.Collections;

public class SpellMeleeHit : SpellTarget
{
	public SpellMeleeHit()
		: base(new Metadata("Coup au corps à corps", "Inflige un coup avec l'arme au corps à corps équipée"))
	{

	}

	public override void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		if (target != null)
		{
			//Vector3 dir = (target.GetCharacterTransform().position - position).normalized;
			target.GetCombatScript().ApplySpell(inflictor, this);
			inflictor.GetCombatScript().Damage(target, inflictor.GetStatsScript().GetDamage());
		}
	}
}
