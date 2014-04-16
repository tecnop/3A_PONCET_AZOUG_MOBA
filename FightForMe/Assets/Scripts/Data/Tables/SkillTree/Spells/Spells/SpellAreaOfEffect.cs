using UnityEngine;
using System.Collections;

public class SpellAreaOfEffect : SpellArea
{
	private float radius;
	private uint impactSpell;
	private float duration;
	private bool onCaster;
	private bool particles;

	public SpellAreaOfEffect(Metadata metadata, float radius, uint impactSpell, float duration = 0.2f, bool onCaster = false, bool particles = false, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f)
		: base(metadata, costType, spellCost)
	{
		this.radius = radius;
		this.impactSpell = impactSpell;
		this.duration = duration;
		this.onCaster = onCaster;
		this.particles = particles;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		if (onCaster)
		{
			inflictor.GetCombatScript().CreateAoE(inflictor.GetCharacterTransform().position, Quaternion.identity, radius, impactSpell, duration, true, particles);
		}
		else
		{
			inflictor.GetCombatScript().CreateAoE(position, Quaternion.identity, radius, impactSpell, duration, false, particles);
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}