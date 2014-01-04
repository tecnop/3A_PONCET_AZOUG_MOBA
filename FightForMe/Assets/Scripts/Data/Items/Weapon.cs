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

	private Weapon() : base()
	{
		this.effectPath = null;
		this.attackSoundPath = null;
		this.projectileID = 0;
		this.skillID = 0;
	}

	// Test constructor
	public Weapon(string name, float damage, float attackRate, uint weaponTypeID) : this()
	{
		this.name = name;
		this.damage = damage;
		this.attackRate = attackRate;
		this.weaponTypeID = weaponTypeID;
	}

	public Weapon(string name,
				  string description,
				  string modelPath,
				  uint weaponTypeID,
				  float damage,
				  float attackRate,
				  uint recyclingXP,
				  uint level,
				  float scale,
				  string effectPath,
				  string iconPath,
				  string attackSoundPath)
	{
		this.name = name;
		this.description = description;
		this.modelPath = modelPath;
		this.weaponTypeID = weaponTypeID;
		this.damage = damage;
		this.attackRate = attackRate;
		this.recyclingXP = recyclingXP;
		this.level = level;
		this.scale = scale;
		this.effectPath = effectPath;
		this.iconPath = iconPath;
		this.attackSoundPath = attackSoundPath;
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
