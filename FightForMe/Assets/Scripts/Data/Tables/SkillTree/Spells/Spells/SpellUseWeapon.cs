using UnityEngine;
using System.Collections;

public class SpellUseWeapon : SpellTarget
{
	//private Spell meleeHit;
	//private Spell projHit;

	public SpellUseWeapon()
		: base(new Metadata("Attaque", "Utilise l'arme équipée"))
	{
		//this.meleeHit = DataTables.GetSpell(2);
		//this.projHit = DataTables.GetSpell(3);
	}

	public override void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		Weapon myWeapon = inflictor.GetInventoryScript().GetWeapon();

		if (myWeapon == null)
		{ // Using our fists
			inflictor.GetCombatScript().CreateAoE(2.0f, 2); // meleeHit
			return;
		}

		WeaponType type = myWeapon.GetWeaponType();
		if (type == null || !type.IsRanged())
		{
			inflictor.GetCombatScript().CreateAoE(2.0f, 2); // meleeHit
		}

		uint proj = myWeapon.GetProjectileID();
		if (proj != 0)
		{
			inflictor.GetCombatScript().CreateProjectile(proj, 3); // projHit
		}
	}
}
