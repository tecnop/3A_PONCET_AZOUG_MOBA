using UnityEngine;
using System.Collections;

public class SpellProjHit : SpellTarget
{
	private float multiplier;

	public SpellProjHit(float multiplier = 1.0f)
		: base(new Metadata("Impact de projectile", "Inflige les dégâts du projectile de l'arme à distance équipée"))
	{
		this.multiplier = multiplier;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		// FIXME: Take the projectile's impact sound instead
		Utils.PlayWeaponSoundOnSource(inflictor.GetInventoryScript().GetWeapon(), true, target.GetAudioSource());

		inflictor.GetCombatScript().Damage(target, inflictor.GetStatsScript().GetProjDamage() * multiplier);
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}
