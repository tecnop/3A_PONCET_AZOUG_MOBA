using UnityEngine;
using System.Collections;

public class SpellKnockback : SpellTarget
{
	public SpellKnockback()
		: base(new Metadata("Expulsion", "Repousse la cible sur une certaine distance"))
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		Vector3 dir = new Vector3(target.GetCharacterTransform().position.x - position.x, 0, target.GetCharacterTransform().position.z - position.z);
		inflictor.GetCombatScript().Knockback(target, dir, 10.0f, 0.75f);
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}
