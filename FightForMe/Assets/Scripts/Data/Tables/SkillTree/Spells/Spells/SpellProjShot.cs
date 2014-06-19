using UnityEngine;
using System.Collections;

public class SpellProjShot : SpellProj
{
	private uint projID;
	private uint amount;
	private uint impactSpellOverride;
	private bool doThrow;
	//private float accuracy;

	public SpellProjShot(Metadata metadata, uint projID, uint amount, float accuracy = 1.0f, uint impactSpellOverride = 0, bool doThrow = false, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f, string castingSound = null)
		: base(metadata, costType, spellCost, castingSound)
	{
		this.projID = projID;
		this.amount = amount;
		this.impactSpellOverride = impactSpellOverride;
		this.doThrow = doThrow;
		//this.accuracy = accuracy;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		uint projID = this.projID;

		if (projID == 0)
		{ // Use our weapon's projectile
			Weapon weapon = inflictor.GetInventoryScript().GetWeapon();
			if (weapon != null)
			{
				projID = weapon.GetProjectileID();
			}
		}

		if (amount > 1)
		{
			Vector3 diff = position - inflictor.GetCharacterTransform().position;
			for (uint i = 0; i < amount; i++)
			{
				float yaw = inflictor.GetCharacterTransform().rotation.eulerAngles.y - 7.5f * amount + 15.0f * i;
				if (amount % 2 != 0) yaw += 7.5f;
				Quaternion angle = Quaternion.Euler(0.0f, yaw, 0.0f);
				ProjectileScript proj = inflictor.GetCombatScript().CreateProjectile(projID, inflictor.GetCharacterTransform().position, angle, impactSpellOverride);
				if (doThrow)
				{
					proj.ThrowAt(inflictor.GetCharacterTransform().position + angle * diff);
				}
			}
		}
		else if (amount == 1)
		{ // The above code works too but is more complicated
			ProjectileScript proj = inflictor.GetCombatScript().CreateProjectile(projID, impactSpellOverride);
			if (doThrow)
			{
				proj.ThrowAt(position);
			}
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		if (this.projID == 0)
		{ // Projectile weapons only
			Weapon weapon = caster.GetInventoryScript().GetWeapon();
			if (weapon != null)
			{
				WeaponType type = weapon.GetWeaponType();
				if (type != null)
				{ // I'm checking this rather than if we have a projectile
					if (type.IsRanged())
					{
						return true;
					}
				}
			}
			return false;
		}
		return true;
	}
}