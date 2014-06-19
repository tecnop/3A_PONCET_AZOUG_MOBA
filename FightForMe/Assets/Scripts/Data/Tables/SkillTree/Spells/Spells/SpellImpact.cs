using UnityEngine;
using System.Collections;

public class SpellImpact : SpellTarget
{
	private float damage;
	private uint buffID;
	private float buffDuration;
	private float knockBackSpeed;
	private float knockBackDuration;
	private bool pull;

	public SpellImpact(Metadata metadata, float damage, uint buffID = 0, float buffDuration = 0.0f, float knockBackSpeed = 0.0f, float knockBackDuration = 0.0f, bool pull = false, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f, string castingSound = null)
		: base(metadata, costType, spellCost, castingSound)
	{
		this.damage = damage;
		this.buffID = buffID;
		this.buffDuration = buffDuration;
		this.knockBackSpeed = knockBackSpeed;
		this.knockBackDuration = knockBackDuration;
		this.pull = pull;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		if (damage > 0.0f)
		{
			inflictor.GetCombatScript().Damage(target, damage);
		}
		if (buffID != 0)
		{
			inflictor.GetCombatScript().InflictBuff(target, buffID, buffDuration);
		}
		if (knockBackSpeed > 0.0f && knockBackDuration > 0.0f)
		{
			Vector3 dir = new Vector3(position.x - target.GetCharacterTransform().position.x, 0, position.z - target.GetCharacterTransform().position.z);
			if (pull)
			{
				inflictor.GetCombatScript().Knockback(target, dir, knockBackSpeed, knockBackDuration);
			}
			else
			{
				inflictor.GetCombatScript().Knockback(target, -dir, knockBackSpeed, knockBackDuration);
			}
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}
