using UnityEngine;
using System.Collections;

public class SpellMeleeHit : SpellTarget
{
	private float multiplier;

	public SpellMeleeHit(float multiplier = 1.0f)
		: base(new Metadata("Coup au corps à corps", "Inflige un coup avec l'arme au corps à corps équipée"))
	{
		this.multiplier = multiplier;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		Utils.PlayWeaponSoundOnSource(inflictor.GetInventoryScript().GetWeapon(), true, target.GetAudioSource());

		inflictor.GetCombatScript().Damage(target, inflictor.GetStatsScript().GetDamage() * multiplier);

		if (inflictor.GetStatsScript().HasSpecialEffect(MiscEffect.FIRE_WEAPON))
		{ // FIXME: This shouldn't be hard coded
			inflictor.GetCombatScript().InflictBuff(target, 3, duration: 5.0f);
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}
