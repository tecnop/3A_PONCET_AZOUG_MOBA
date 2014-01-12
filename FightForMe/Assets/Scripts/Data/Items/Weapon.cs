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
		uint recyclingXP = 0,
		uint level = 0,
		uint skillID = 0,
		uint weaponTypeID = 0,
		float damage = 0.0f,
		float attackRate = 1.0f,
		float scale = 1.0f,
		string effectPath = null,
		string attackSoundPath = null)
		: base(name, description, modelPath, iconPath, recyclingXP, level, skillID)
	{
		this.weaponTypeID = weaponTypeID;
		this.damage = damage;
		this.attackRate = attackRate;
		this.scale = scale;
		this.effectPath = effectPath;
		this.attackSoundPath = attackSoundPath;
		this.iAmAWeapon = true;
	}

	public float getDamage() { return this.damage; }
	public float getAttackRate() { return this.attackRate; }

	public string getEffect() { return this.effectPath; }
	public string getAttackSound() { return this.attackSoundPath; }

	public WeaponType getWeaponType() { return DataTables.getWeaponType(this.weaponTypeID); }
	//public Projectile getProjectile() {return DataTables.getProjectile(this.projectileID);}
	//public Skill getWeaponSkill() {return DataTables.getSkill(this.skillID);}

	public float getScale() { return this.scale; }
}
