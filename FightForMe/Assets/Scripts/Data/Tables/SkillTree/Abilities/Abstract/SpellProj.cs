using UnityEngine;
using System.Collections;

public abstract class SpellProj: Ability
{
	protected GameObject ShootProjectile(CharacterManager owner, Vector3 position, Quaternion angle, uint projectileID, CharacterManager target = null)
	{ // Necessary?
		return null;
	}

	public SpellProj(Metadata metadata)
		: base(metadata, AbilityType.PROJECTILE)
	{

	}
}