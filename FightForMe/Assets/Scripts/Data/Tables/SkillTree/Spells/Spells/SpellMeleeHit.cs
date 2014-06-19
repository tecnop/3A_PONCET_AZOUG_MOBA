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
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}
