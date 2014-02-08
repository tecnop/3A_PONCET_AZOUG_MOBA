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

	private float scale;				// Model and hitbox scale

	public Weapon(string name = "Arme",
		string description = null,
		string modelPath = null,
		string iconPath = null,
		uint recyclingXP = 100,
		uint level = 0,
		uint skillID = 0,
		uint weaponTypeID = 0,
		float damage = 0.0f,
		float attackRate = 1.0f,
		float scale = 1.0f,
		uint projectileID = 0,
		string effectPath = null,
		string attackSoundPath = null)
		: base(name, description, modelPath, iconPath, recyclingXP, level, skillID, true)
	{
		this.weaponTypeID = weaponTypeID;
		this.damage = damage;
		this.attackRate = attackRate;
		this.scale = scale;
		this.projectileID = projectileID;
		this.effectPath = effectPath;
		this.attackSoundPath = attackSoundPath;
	}

	public float GetDamage() { return this.damage; }
	public float GetAttackRate() { return this.attackRate; }

	public string GetEffect() { return this.effectPath; }
	public string GetAttackSound() { return this.attackSoundPath; }

	public WeaponType GetWeaponType() { return DataTables.GetWeaponType(this.weaponTypeID); }
	public Projectile GetProjectile() {return DataTables.GetProjectile(this.projectileID);}

	public float GetScale() { return this.scale; }
}
