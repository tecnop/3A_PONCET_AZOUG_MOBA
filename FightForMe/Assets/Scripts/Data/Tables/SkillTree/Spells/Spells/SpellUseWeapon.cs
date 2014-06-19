using UnityEngine;
using System.Collections;

public class SpellUseWeapon : SpellTarget
{
	private uint meleeHit;
	private uint rangedHit;

	public SpellUseWeapon(uint meleeHit = 2, uint rangedHit = 3)
		: base(new Metadata("Attaque", "Utilise l'arme équipée"), castingSound:"null") // Override the sound because this spell plays one itself
	{
		this.meleeHit = meleeHit;
		this.rangedHit = rangedHit;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		Weapon myWeapon = inflictor.GetInventoryScript().GetWeapon();

		Utils.PlayWeaponSoundOnSource(myWeapon, false, inflictor.GetAudioSource());

		if (myWeapon == null)
		{ // Using our fists
			inflictor.GetCombatScript().CreateAoE(2.0f, this.meleeHit);
			return;
		}

		WeaponType type = myWeapon.GetWeaponType();
		if (type == null || !type.IsRanged())
		{
			inflictor.GetCombatScript().CreateAoE(2.0f, this.meleeHit);
		}

		uint proj = myWeapon.GetProjectileID();
		if (proj != 0)
		{
			inflictor.GetCombatScript().CreateProjectile(proj, this.rangedHit);
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}
