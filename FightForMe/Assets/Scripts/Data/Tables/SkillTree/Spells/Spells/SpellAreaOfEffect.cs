using UnityEngine;
using System.Collections;

public class SpellAreaOfEffect : SpellArea
{
	private float radius;
	private uint impactSpell;
	private float duration;
	private bool onCaster;

	public SpellAreaOfEffect(Metadata metadata, float radius, uint impactSpell, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f, float duration = 0.2f, bool onCaster = false)
		: base(metadata, costType, spellCost)
	{
		this.radius = radius;
		this.impactSpell = impactSpell;
		this.duration = duration;
		this.onCaster = onCaster;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		if (onCaster)
		{
			inflictor.GetCombatScript().CreateAoE(inflictor.GetCharacterTransform().position, Quaternion.identity, radius, impactSpell, duration, true);
		}
		else
		{
			inflictor.GetCombatScript().CreateAoE(position, Quaternion.identity, radius, impactSpell, duration);
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}