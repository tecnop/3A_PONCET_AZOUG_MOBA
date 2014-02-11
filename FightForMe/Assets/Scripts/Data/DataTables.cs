using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		// 2 - Abilities
		// 3 - Status changes (AKA effects)
		// 4 - Buffs
		// 5 - Skill tree
		// 6 - Projectiles
		// 7 - Weapon types, armor sets
		// 8 - Items
		// 9 - Monsters
		// It doesn't always matter because we mostly use IDs (for now), but for the sake of consistency this order should be used
		
		uint[] temp;
		
		// Effects
		effectTable.Add(1, new Effect(description: "Stats du seigneur", isPositive: true, flatHP: 2000, stats: new Stats(50, 50, 50)));
		effectTable.Add(2, new Effect(description: "Capacité: Boule de feu", isPositive: true, unlockedAbility:1));
		effectTable.Add(3, new Effect(description: "Endurance", isPositive: true, stats:new Stats(strength:10)));
		effectTable.Add(4, new Effect(description: "Puissance", isPositive: true, stats: new Stats(agility: 10)));
		effectTable.Add(5, new Effect(description: "Intelligence", isPositive: true, stats: new Stats(intelligence: 10)));
		effectTable.Add(6, new Effect(description: "Endurance+", isPositive: true, stats: new Stats(strength: 30)));
		effectTable.Add(7, new Effect(description: "Puissance+", isPositive: true, stats: new Stats(agility: 30)));
		effectTable.Add(8, new Effect(description: "Intelligence+", isPositive: true, stats: new Stats(intelligence: 30)));
		effectTable.Add(9, new Effect(description: "Vitesse de Course", isPositive: true, flatMS: 350.0f));
		effectTable.Add(10, new Effect(description: "Vitesse de Course+", isPositive: true, flatMS: 350.0f));
		effectTable.Add(11, new Effect(description: "Super Debug", isPositive: true, stats: new Stats(200, 200, 200), bonusDamage: 1000.0f));

		// Buffs
		temp = new uint[1];
		temp[0] = 1;
		buffTable.Add(1, new Buff(name: "Seigneur", effects: temp));
		
		temp = new uint[1];
		temp[0] = 11;
		buffTable.Add(2, new Buff(name: "Testeur", effects: temp));
		
		// Skills
		temp = new uint[]{2,3,4,8};
		skillTable.Add(1, new Skill(name: "Première compétence", neighbours:temp));
		
		temp = new uint[2];
		temp[0] = 1;
		temp[1] = 5;
		skillTable.Add(2, new Skill(name: "Bonus d'endurance", color: SkillColor.R, effect: 3, neighbours: temp));
		temp[0] = 1;
		temp[1] = 6;
		skillTable.Add(3, new Skill(name: "Bonus de puissance", color: SkillColor.G, effect: 4, neighbours: temp));
		temp[0] = 1;
		temp[1] = 7;
		skillTable.Add(4, new Skill(name: "Bonus d'intelligence", color: SkillColor.B, effect: 5, neighbours: temp));
		temp = new uint[1];
		temp[0] = 2;
		skillTable.Add(5, new Skill(name: "Super bonus d'endurance", color: SkillColor.R, effect: 6, neighbours: temp));
		temp[0] = 3;
		skillTable.Add(6, new Skill(name: "Super bonus de puissance", color: SkillColor.G, effect: 7, neighbours: temp));
		temp[0] = 4;
		skillTable.Add(7, new Skill(name: "Super bonus d'intelligence", color: SkillColor.B, effect: 8, neighbours: temp));

		temp = new uint[2];
		temp[0] = 1;
		temp[1] = 9;
		skillTable.Add(8, new Skill(name: "Bonus vitesse de Course", color: SkillColor.B, effect: 9, neighbours: temp));

		temp = new uint[1];
		temp[0] = 8;
		skillTable.Add(9, new Skill(name: "Super bonus vit. de Course", color: SkillColor.B, effect: 10, neighbours: temp));
		
		// Projectiles
		projectileTable.Add(1, new Projectile(name: "Flèche du seigneur", damage: 30.0f, speed: 50.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(2, new Projectile(name: "Boule de feu", damage: 15.0f, speed: 10.0f, hitboxSize: new Vector3(0.5f, 0.5f, 0.5f), impactRadius: 2.0f));
		projectileTable.Add(3, new Projectile(name: "Balle", damage: 50.0f, speed: 150.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(4, new Projectile(name: "Flèche", damage: 10.0f, speed: 50.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		
		// Weapon types
		weaponTypeTable.Add(1, new WeaponType(name: "Epee courte"));
		weaponTypeTable.Add(2, new WeaponType(name: "Marteau"));
		weaponTypeTable.Add(3, new WeaponType(name: "Arc", isRanged:true));
		
		// Armor sets
		armorSetTable.Add(1, new ArmorSet(name: "Panoplie du Seigneur", buffID: 1));
		armorSetTable.Add(2, new ArmorSet(name: "Le Beta testeur", buffID: 2));
		
		// Items
		itemTable.Add(1, new Weapon(name: "Epee des mile phote d'ortograff", damage: 20.0f, attackRate: 1.0f));
		itemTable.Add(2, new Armor(name: "Armure du test ultime", slot: ArmorSlot.TORSO, setID: 2));
		itemTable.Add(3, new Weapon(name: "La Dague", damage: 6.66f, attackRate: 1.5f));
		itemTable.Add(4, new Weapon(name: "Marteau du Seigneur", damage: 50.0f, attackRate: 1.0f, weaponTypeID: 2));
		itemTable.Add(5, new Weapon(name: "Arc du Seigneur", attackRate: 0.5f, weaponTypeID: 3, projectileID: 1));
		itemTable.Add(6, new Armor(name: "Armure du Seigneur", slot: ArmorSlot.TORSO, setID: 1));
		itemTable.Add(7, new Armor(name: "Casque du Seigneur", slot: ArmorSlot.HEAD, setID: 1));
		itemTable.Add(8, new Armor(name: "Bottes du Seigneur", slot: ArmorSlot.FEET, setID: 1));
		itemTable.Add(9, new Weapon(name: "Le fouetteur d'Yggtralala", damage: 15.0f, attackRate: 1.0f));
		itemTable.Add(10, new Armor(name: "Casque du super debug", slot: ArmorSlot.HEAD, setID: 2));
		itemTable.Add(11, new Armor(name: "Gants de l'incroyable fix", slot: ArmorSlot.HANDS, setID:2));
		itemTable.Add(12, new Armor(name: "Bottes de l'interminable alpha", slot: ArmorSlot.FEET, setID:2));
		itemTable.Add(13, new Weapon(name: "Croc de la Téci", damage: 6.0f, attackRate: 2.0f));
		itemTable.Add(14, new Weapon(name: "La quat'cinq", attackRate: 0.3f, weaponTypeID: 3, projectileID: 3));
		itemTable.Add(15, new Weapon(name: "Arc biodégradable", attackRate: 1.0f, weaponTypeID: 3, projectileID: 4));
		itemTable.Add(16, new Weapon(name: "Pierre à XP", recyclingXP:1000));
		
		// Monsters
		temp = new uint[1];
		temp[0] = 9;
		monsterTable.Add(1, new Monster(name: "Zombie", behaviour: AIType.defensive, modelPath: "Cylinder", items: temp));
		
		temp = new uint[5];
		temp[0] = 4;
		temp[1] = 5;
		temp[2] = 6;
		temp[3] = 7;
		temp[4] = 8;
		// ============= HARD-CODED REFERENCE =============
		monsterTable.Add(2, new Monster(name: "Lord", behaviour: AIType.defensive, items: temp));
		// ============= HARD-CODED REFERENCE =============
		
		temp = new uint[1];
		temp[0] = 13;
		monsterTable.Add(3, new Monster(name: "Ratus", behaviour: AIType.defensive, items: temp));
		
		temp[0] = 15;
		monsterTable.Add(4, new Monster(name: "Archet", behaviour: AIType.defensive, items: temp));
		
		temp[0] = 14;
		monsterTable.Add(5, new Monster(name: "Snaille'p", behaviour: AIType.defensive, items: temp));

		// Debug Monsters
		temp = new uint[2];
		temp[0] = 2;
		temp[1] = 10;
		monsterTable.Add(6, new Monster(name: "Debug1", behaviour: AIType.defensive, items: temp));
		temp[0] = 11;
		temp[1] = 12;
		monsterTable.Add(7, new Monster(name: "Debug2", behaviour: AIType.defensive, items: temp));
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
		if (modelTable.ContainsKey(name))
		{
			return modelTable[name];
		}
		return null;
	}
}
