using UnityEngine;
using System.Collections;

public static class Utils
{
	public static Vector3 DiffNoY(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x - b.x, 0, a.z - b.z);
	}

	public static Vector2 MousePosToScreenPos(Vector3 mousePos)
	{
		return new Vector2(mousePos.x, Screen.height - mousePos.y);
	}

	public static void PlaySpellSoundOnSource(Spell spell, AudioSource source)
	{
		AudioClip sound = DataTables.GetSound(spell.GetCastingSound());

		if (sound != null)
		{
			source.PlayOneShot(sound);
		}
	}

	public static void PlayWeaponSoundOnSource(Weapon weapon, bool isImpact, AudioSource source)
	{
		AudioClip sound;

		if (weapon == null)
		{ // Ooooo
			if (isImpact)
			{
				sound = DataTables.GetSound("punchhit");
			}
			else
			{
				sound = DataTables.GetSound("punchswing");
			}
		}
		else
		{
			if (isImpact)
			{
				sound = DataTables.GetSound(weapon.GetHitSound());
			}
			else
			{
				sound = DataTables.GetSound(weapon.GetAttackSound());
			}
		}

		if (sound != null)
		{
			source.PlayOneShot(sound);
		}
	}
}
