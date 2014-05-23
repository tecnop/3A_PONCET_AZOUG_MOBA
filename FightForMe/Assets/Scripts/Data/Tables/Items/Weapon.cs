using UnityEngine;
using System.Collections;

public class Weapon : Item
{
	private float damage;				// Damage inflicted per hit
	private float attackRate;			// Amount of attacks per second

	private string effectPath;			// Particle effect to be played on the weapon
	private string attackSoundPath;		// Sound to be played on weapon swing

	private uint weaponTypeID;			// ID of the entry in the weapon type table this weapon matches
	private uint projectileID;			// ID of the entry in the projectile table this weapon should shoot when swung

	public override WikiCategory category
	{
		get
		{
			return WikiCategory.ITEMS;
		}
	}

	public Weapon(Metadata metadata,
		uint recyclingXP = 100,
		uint buffID = 0,
		uint weaponTypeID = 0,
		float damage = 0.0f,
		float attackRate = 1.0f,
		uint projectileID = 0,
		string effectPath = null,
		string attackSoundPath = null)
		: base(metadata, recyclingXP, buffID, true)
	{
		this.weaponTypeID = weaponTypeID;
		this.damage = damage;
		this.attackRate = attackRate;
		this.projectileID = projectileID;
		this.effectPath = effectPath;
		this.attackSoundPath = attackSoundPath;
	}

	public float GetDamage() { return this.damage; }
	public float GetAttackRate() { return this.attackRate; }

	public string GetEffect() { return this.effectPath; }
	public string GetAttackSound() { return this.attackSoundPath; }

	public WeaponType GetWeaponType() { return DataTables.GetWeaponType(this.weaponTypeID); }
	public uint GetProjectileID() { return this.projectileID; }
	public Projectile GetProjectile() { return DataTables.GetProjectile(this.projectileID); }

	public override void DrawDataWindow(float width, float height)
	{
		base.DrawDataWindow(width, height);

		if (this.weaponTypeID != 0)
		{
			GUI.BeginGroup(SRect.Make(2.0f * width / 3.0f, 0.0f, width / 3.0f, height / 5.0f, "data_window_weapon_type"));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Type:");
			WikiManager.DrawReferenceInLayout(DataTables.GetWeaponType(this.weaponTypeID));
			GUILayout.EndHorizontal();
			GUI.EndGroup();
		}

		if (this.projectileID != 0)
		{
			GUI.BeginGroup(SRect.Make(2.0f * width / 3.0f, height / 5.0f, width / 3.0f, 2.0f * height / 5.0f, "data_window_weapon_proj"));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Tire:");
			WikiManager.DrawReferenceInLayout(DataTables.GetProjectile(this.projectileID));
			GUILayout.EndHorizontal();
			GUI.EndGroup();
		}

		GUI.Label(SRect.Make(10.0f, 0.45f * height, width - 20.0f, 0.55f * height - 40.0f, "data_window_weapon_stats"), "Inflige " + this.damage + " dégâts " + this.attackRate + " fois par seconde");
	}

	public override void DrawWikiPage(float width, float height)
	{
		base.DrawWikiPage(width, height);

		/*if (this.weaponTypeID != 0)
		{
			GUI.BeginGroup(SRect.Make(2.0f * width / 3.0f, 0.0f, width / 3.0f, height / 5.0f, "wiki_weapon_type"));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Type:");
			WikiManager.DrawReferenceInLayout(DataTables.GetWeaponType(this.weaponTypeID));
			GUILayout.EndHorizontal();
			GUI.EndGroup();
		}*/

		if (this.projectileID != 0)
		{
			GUI.BeginGroup(SRect.Make(10.0f, 50.0f, width / 3.0f, height / 3.0f, "wiki_weapon_proj"));
			GUILayout.BeginVertical();
			GUILayout.Label("Tire:", FFMStyles.centeredText);
			WikiManager.DrawReferenceInLayout(DataTables.GetProjectile(this.projectileID));
			GUILayout.EndVertical();
			GUI.EndGroup();
		}
	}
}
