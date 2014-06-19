using UnityEngine;
using System.Collections;

public class SpellDash : SpellTarget
{
	private float speed;
	private float duration;
	private bool brake;
	private uint impactSpell;

	public SpellDash(Metadata metadata, float speed, float duration, bool brake = false, uint impactSpell = 0, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f, string castingSound = null)
		: base(metadata, costType, spellCost, castingSound)
	{
		this.speed = speed;
		this.duration = duration;
		this.brake = brake;
		this.impactSpell = impactSpell;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		Vector3 dir = new Vector3(position.x - target.GetCharacterTransform().position.x, 0, position.z - target.GetCharacterTransform().position.z);
		if (this.brake)
		{
			inflictor.GetCombatScript().Knockback(target, dir, speed, duration);
		}
		else
		{
			target.GetMovementScript().SetMovementOverride(dir, speed, duration, false);
		}

		if (this.impactSpell != 0)
		{ // TODO: Variable radius?
			target.GetCombatScript().CreateAoE(2.0f, impactSpell, brake?duration/2.0f:duration);
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{ // TODO: Shield
		return true;
	}
}