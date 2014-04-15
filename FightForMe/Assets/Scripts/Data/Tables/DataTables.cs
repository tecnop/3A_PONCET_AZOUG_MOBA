using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class DataTables
{ // NOTE: All table IDs start at 1, 0 is used as a null value
	// Entities
	static Dictionary<uint, Monster> monsterTable = new Dictionary<uint, Monster>();
	static Dictionary<uint, Projectile> projectileTable = new Dictionary<uint, Projectile>();

	// Items
	static Dictionary<uint, Item> itemTable = new Dictionary<uint, Item>();
	static Dictionary<uint, WeaponType> weaponTypeTable = new Dictionary<uint, WeaponType>();
	static Dictionary<uint, ArmorSet> armorSetTable = new Dictionary<uint, ArmorSet>();

	// Skill Tree
	static Dictionary<uint, Skill> skillTable = new Dictionary<uint, Skill>();

	// Spells
	static Dictionary<uint, Spell> spellTable = new Dictionary<uint, Spell>();

	// Buffs
	static Dictionary<uint, Effect> effectTable = new Dictionary<uint, Effect>();
	static Dictionary<uint, Buff> buffTable = new Dictionary<uint, Buff>();

	// Resources
	static Dictionary<string, GameObject> modelTable = new Dictionary<string, GameObject>();

	private static void clearTables()
	{
		monsterTable.Clear();
		projectileTable.Clear();

		itemTable.Clear();
		weaponTypeTable.Clear();
		armorSetTable.Clear();

		skillTable.Clear();

		spellTable.Clear();

		effectTable.Clear();
		buffTable.Clear();

		// Not clearing resource tables for now
		//modelTable.Clear();
	}

	public static void LoadModels(GameObject[] models)
	{
		modelTable.Clear(); // Temporary

		foreach (GameObject obj in models)
		{
			modelTable.Add(obj.name, obj);
		}
	}

	public static void updateTables()
	{
		clearTables();
		// NOTE: To account for dependencies, the tables should be initialized in the following order:
		// 1 - Resources
		// 2 - Spells
		// 3 - Status changes (AKA effects)
		// 4 - Buffs
		// 5 - Skill tree	// NOTE: Place before status changes so we can build effect descriptions automatically?
		// 6 - Projectiles
		// 7 - Weapon types, armor sets
		// 8 - Items
		// 9 - Monsters

		// Spells
		// ============= HARD-CODED REFERENCES =============
		spellTable.Add(1, new SpellUseWeapon());
		spellTable.Add(2, new SpellMeleeHit());
		spellTable.Add(3, new SpellProjHit());
		// ================================================
		spellTable.Add(4, new SpellFireball());
		spellTable.Add(5, new SpellFireExplosion());
		spellTable.Add(6, new SpellBurn());
		spellTable.Add(7, new SpellGrenade());
		spellTable.Add(8, new SpellExplosion());
		spellTable.Add(9, new SpellKnockback());
		spellTable.Add(10, new SpellProjShot(new Metadata("Multishot"), 4, 5));
		spellTable.Add(11, new SpellDash(new Metadata("Charge"), 30.0f, 1.0f, true, impactSpell: 12));
		spellTable.Add(12, new SpellImpact(new Metadata("Charge impact"), 15, 0, 0.0f, 30.0f, 2.0f));
		spellTable.Add(13, new SpellToggleBuff(new Metadata("Self burn"), 3));

		// Effects
		effectTable.Add(1, new Effect(description: "Stats du seigneur", isPositive: true, flatHP: 2000, stats: new Stats(50, 50, 50)));
		effectTable.Add(2, new Effect(description: "Capacité: Boule de feu", isPositive: true, unlockedAbility: 1));
		effectTable.Add(3, new Effect(description: "Endurance", isPositive: true, stats: new Stats(strength: 10)));
		effectTable.Add(4, new Effect(description: "Puissance", isPositive: true, stats: new Stats(agility: 10)));
		effectTable.Add(5, new Effect(description: "Intelligence", isPositive: true, stats: new Stats(intelligence: 10)));
		effectTable.Add(6, new Effect(description: "Endurance+", isPositive: true, stats: new Stats(strength: 30)));
		effectTable.Add(7, new Effect(description: "Puissance+", isPositive: true, stats: new Stats(agility: 30)));
		effectTable.Add(8, new Effect(description: "Intelligence+", isPositive: true, stats: new Stats(intelligence: 30)));
		effectTable.Add(9, new Effect(description: "Vitesse de Course", isPositive: true, flatMS: 350.0f));
		effectTable.Add(10, new Effect(description: "Vitesse de Course+", isPositive: true, flatMS: 350.0f));
		effectTable.Add(11, new Effect(description: "Super Debug", isPositive: true, stats: new Stats(200, 200, 200), bonusDamage: 1000.0f, bonusProjDamage: 1000.0f));
		effectTable.Add(12, new Effect(description: "Brûlure", isPositive: false, pctHPRegen: -0.05f));
		effectTable.Add(13, new Effect(description: "Boule de Feu", isPositive: true, unlockedAbility: 4));
		// ============= HARD-CODED REFERENCE =============
		effectTable.Add(14, new Effect(description: "Attaque", isPositive: true, unlockedAbility: 1));
		// ================================================
		effectTable.Add(15, new Effect(description: "Grenade", isPositive: true, unlockedAbility: 7));
		// ============= HARD-CODED REFERENCE =============
		effectTable.Add(16, new Effect(description: "Relique", isPositive: true, misc: MiscEffect.CARRYING_TROPHY));
		// ================================================
		effectTable.Add(17, new Effect(description: "Tir multiple", isPositive: true, unlockedAbility: 10));
		effectTable.Add(18, new Effect(description: "Charge", isPositive: true, unlockedAbility: 11));
		effectTable.Add(19, new Effect(description: "Auto-Brûlure", isPositive: true, unlockedAbility: 13));

		// Buffs
		buffTable.Add(1, new Buff(metadata: new Metadata(name: "Seigneur"), effects: new uint[] { 1 }));

		buffTable.Add(2, new Buff(metadata: new Metadata(name: "Testeur"), effects: new uint[] { 11 }));

		buffTable.Add(3, new Buff(metadata: new Metadata(name: "Brûlure"), effects: new uint[] { 12 }));

		buffTable.Add(4, new Buff(metadata: new Metadata(name: "Relique"), effects: new uint[] { 16 }));

		// Skills
		// ============= HARD-CODED REFERENCE =============
		skillTable.Add(1, new Skill(metadata: new Metadata(name: "Première compétence"), effect: 14, neighbours: new uint[] { 2, 3, 4, 8 }));
		// ================================================

		skillTable.Add(2, new Skill(metadata: new Metadata(name: "Bonus d'endurance"), color: SkillColor.R, effect: 3, neighbours: new uint[2] { 1, 5 }));
		skillTable.Add(3, new Skill(metadata: new Metadata(name: "Bonus de puissance"), color: SkillColor.G, effect: 4, neighbours: new uint[] { 1, 6 }));
		skillTable.Add(4, new Skill(metadata: new Metadata(name: "Bonus d'intelligence"), color: SkillColor.B, effect: 5, neighbours: new uint[] { 1, 7 }));

		skillTable.Add(5, new Skill(metadata: new Metadata(name: "Super bonus d'endurance"), color: SkillColor.R, effect: 6, neighbours: new uint[] { 2, 13 }));
		skillTable.Add(6, new Skill(metadata: new Metadata(name: "Super bonus de puissance"), color: SkillColor.G, effect: 7, neighbours: new uint[] { 3, 12 }));
		skillTable.Add(7, new Skill(metadata: new Metadata(name: "Super bonus d'intelligence"), color: SkillColor.B, effect: 8, neighbours: new uint[] { 4, 10 }));

		skillTable.Add(8, new Skill(metadata: new Metadata(name: "Bonus vitesse de Course"), color: SkillColor.W, effect: 9, neighbours: new uint[] { 1, 9 }));
		skillTable.Add(9, new Skill(metadata: new Metadata(name: "Super bonus vit. de Course"), color: SkillColor.W, effect: 10, neighbours: new uint[] { 8 }));

		skillTable.Add(10, new Skill(metadata: new Metadata(name: "Sort: Boule de Feu"), color: SkillColor.B, effect: 13, neighbours: new uint[] { 7, 11 }));
		skillTable.Add(11, new Skill(metadata: new Metadata(name: "Sort: Grenade"), color: SkillColor.B, effect: 15, neighbours: new uint[] { 10 }));
		skillTable.Add(12, new Skill(metadata: new Metadata(name: "Sort: Multi-tir"), color: SkillColor.G, effect: 17, neighbours: new uint[] { 6 }));
		skillTable.Add(13, new Skill(metadata: new Metadata(name: "Sort: Charge"), color: SkillColor.R, effect: 18, neighbours: new uint[] { 5, 14 }));
		skillTable.Add(14, new Skill(metadata: new Metadata(name: "Sort: Auto-Brûlure"), color: SkillColor.R, effect: 19, neighbours: new uint[] { 13 }));

		// Projectiles
		projectileTable.Add(1, new Projectile(metadata: new Metadata(name: "Flèche du seigneur"), damage: 30.0f, speed: 50.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(2, new Projectile(metadata: new Metadata(name: "Boule de feu"), damage: 15.0f, speed: 10.0f, hitboxSize: new Vector3(0.5f, 0.5f, 0.5f), impactAbility: 5));
		projectileTable.Add(3, new Projectile(metadata: new Metadata(name: "Balle"), damage: 50.0f, speed: 150.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(4, new Projectile(metadata: new Metadata(name: "Flèche"), damage: 10.0f, speed: 50.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(5, new Projectile(metadata: new Metadata(name: "Grenade"), damage: 15.0f, speed: 10.0f, hitboxSize: new Vector3(0.5f, 0.5f, 0.5f), impactAbility: 8, trajectory: ProjectileTrajectory.Throw, lifeTime: 0.0f));

		// Weapon types
		weaponTypeTable.Add(1, new WeaponType(metadata: new Metadata(name: "Epee courte")));
		weaponTypeTable.Add(2, new WeaponType(metadata: new Metadata(name: "Marteau"), isTwoHanded: true));
		weaponTypeTable.Add(3, new WeaponType(metadata: new Metadata(name: "Arc"), isRanged: true, isTwoHanded: true));

		// Armor sets
		armorSetTable.Add(1, new ArmorSet(metadata: new Metadata(name: "Panoplie du Seigneur"), buffID: 1));
		armorSetTable.Add(2, new ArmorSet(metadata: new Metadata(name: "Le Beta testeur"), buffID: 2));

		// Items
		itemTable.Add(1, new Weapon(metadata: new Metadata(name: "Epee des mile phote d'ortograff"), damage: 20.0f, attackRate: 1.0f));
		itemTable.Add(2, new Armor(metadata: new Metadata(name: "Armure du test ultime"), slot: ArmorSlot.TORSO, setID: 2));
		itemTable.Add(3, new Weapon(metadata: new Metadata(name: "La Dague"), damage: 6.66f, attackRate: 1.5f));
		itemTable.Add(4, new Weapon(metadata: new Metadata(name: "Marteau du Seigneur"), damage: 50.0f, attackRate: 1.0f, weaponTypeID: 2));
		itemTable.Add(5, new Weapon(metadata: new Metadata(name: "Arc du Seigneur"), attackRate: 0.5f, weaponTypeID: 3, projectileID: 1));
		itemTable.Add(6, new Armor(metadata: new Metadata(name: "Armure du Seigneur"), slot: ArmorSlot.TORSO, setID: 1));
		itemTable.Add(7, new Armor(metadata: new Metadata(name: "Casque du Seigneur"), slot: ArmorSlot.HEAD, setID: 1));
		itemTable.Add(8, new Armor(metadata: new Metadata(name: "Bottes du Seigneur"), slot: ArmorSlot.FEET, setID: 1));
		itemTable.Add(9, new Weapon(metadata: new Metadata(name: "Le fouetteur d'Yggtralala"), damage: 15.0f, attackRate: 1.0f));
		itemTable.Add(10, new Armor(metadata: new Metadata(name: "Casque du super debug"), slot: ArmorSlot.HEAD, setID: 2));
		itemTable.Add(11, new Armor(metadata: new Metadata(name: "Gants de l'incroyable fix"), slot: ArmorSlot.HANDS, setID: 2));
		itemTable.Add(12, new Armor(metadata: new Metadata(name: "Bottes de l'interminable alpha"), slot: ArmorSlot.FEET, setID: 2));
		itemTable.Add(13, new Weapon(metadata: new Metadata(name: "Croc de la Téci"), damage: 6.0f, attackRate: 2.0f));
		itemTable.Add(14, new Weapon(metadata: new Metadata(name: "La quat'cinq"), attackRate: 0.3f, weaponTypeID: 3, projectileID: 3));
		itemTable.Add(15, new Weapon(metadata: new Metadata(name: "Arc biodégradable"), attackRate: 1.0f, weaponTypeID: 3, projectileID: 4));
		itemTable.Add(16, new Weapon(metadata: new Metadata(name: "Pierre à XP"), recyclingXP: 1000));

		// Monsters

		// ============= HARD-CODED REFERENCE =============
		monsterTable.Add(1, new Monster(metadata: new Metadata(name: "Lord"), behaviour: AIType.defensive, items: new uint[] { 4, 5, 6, 7, 8 }));
		// ================================================

		monsterTable.Add(2, new Monster(metadata: new Metadata(name: "Zombie", modelPath: "Cylinder"), behaviour: AIType.defensive, items: new uint[] { 9 }));
		monsterTable.Add(3, new Monster(metadata: new Metadata(name: "Ratus"), behaviour: AIType.defensive, items: new uint[] { 13 }));
		monsterTable.Add(4, new Monster(metadata: new Metadata(name: "Archet"), behaviour: AIType.defensive, items: new uint[] { 15 }));
		monsterTable.Add(5, new Monster(metadata: new Metadata(name: "Snaille'p"), behaviour: AIType.defensive, items: new uint[] { 14 }));

		// Debug Monsters
		monsterTable.Add(6, new Monster(metadata: new Metadata(name: "Debug1"), behaviour: AIType.defensive, items: new uint[] { 2, 10 }));
		monsterTable.Add(7, new Monster(metadata: new Metadata(name: "Debug2"), behaviour: AIType.defensive, items: new uint[] { 11, 12 }));

		// ============= HARD-CODED REFERENCE =============
		monsterTable.Add(8, new Monster(metadata: new Metadata(name: "Hasnor", scale: 5.0f, quality: Quality.UNIQUE), behaviour: AIType.aggressive, items: new uint[] { 4, 5, 6, 7, 8 }));
		// ================================================
	}

	public static Item GetItem(uint key)
	{
		if (itemTable.ContainsKey(key))
		{
			return itemTable[key];
		}
		return null;
	}

	public static WeaponType GetWeaponType(uint key)
	{
		if (weaponTypeTable.ContainsKey(key))
		{
			return weaponTypeTable[key];
		}
		return null;
	}

	public static Monster GetMonster(uint key)
	{
		if (monsterTable.ContainsKey(key))
		{
			return monsterTable[key];
		}
		return null;
	}

	public static ArmorSet GetArmorSet(uint key)
	{
		if (armorSetTable.ContainsKey(key))
		{
			return armorSetTable[key];
		}
		return null;
	}

	public static Projectile GetProjectile(uint key)
	{
		if (projectileTable.ContainsKey(key))
		{
			return projectileTable[key];
		}
		return null;
	}

	public static Skill GetSkill(uint key)
	{
		if (skillTable.ContainsKey(key))
		{
			return skillTable[key];
		}
		return null;
	}

	public static Spell GetSpell(uint key)
	{
		if (spellTable.ContainsKey(key))
		{
			return spellTable[key];
		}
		return null;
	}

	public static Buff GetBuff(uint key)
	{
		if (buffTable.ContainsKey(key))
		{
			return buffTable[key];
		}
		return null;
	}

	public static Effect GetEffect(uint key)
	{
		if (effectTable.ContainsKey(key))
		{
			return effectTable[key];
		}
		return null;
	}

	public static GameObject GetModel(string name)
	{
		if (name != null && modelTable.ContainsKey(name))
		{
			return modelTable[name];
		}
		return null;
	}
}
