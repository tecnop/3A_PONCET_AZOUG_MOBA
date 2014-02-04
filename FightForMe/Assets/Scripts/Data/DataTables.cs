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
		// 2 - Status changes (AKA effects)
		// 3 - Buffs
		// 4 - Skill tree
		// 5 - Projectiles
		// 6 - Weapon types, armor sets
		// 7 - Items
		// 8 - Monsters
		// It doesn't always matter because we mostly use IDs (for now), but for the sake of consistency this order should be used

		uint[] temp;

		// Effects
		effectTable.Add(1, new Effect(description: "Stats du seigneur", isPositive: true, flatHP: 2000, stats: new Stats(50, 50, 50)));

		// Buffs
		temp = new uint[1];
		temp[0] = 1;
		buffTable.Add(1, new Buff(name: "Seigneur", effects: temp));

		// Skills

		// Projectiles
		projectileTable.Add(1, new Projectile(name: "Flèche du seigneur", damage: 30.0f, speed: 5.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(2, new Projectile(name: "Boule de feu", damage: 15.0f, speed: 1.0f, hitboxSize: new Vector3(0.5f, 0.5f, 0.5f), impactRadius: 2.0f));

		// Weapon types
		weaponTypeTable.Add(1, new WeaponType(name: "Epee courte"));
		weaponTypeTable.Add(2, new WeaponType(name: "Marteau"));
		weaponTypeTable.Add(3, new WeaponType(name: "Arc"));

		// Armor sets
		armorSetTable.Add(1, new ArmorSet(name: "Panoplie du Seigneur", buffID: 1));

		// Items
		itemTable.Add(1, new Weapon(name: "Epee des mile phote d'ortograff", damage: 10.0f, attackRate: 1.0f));
		itemTable.Add(2, new Armor(name: "Armure du test ultime"));
		itemTable.Add(3, new Weapon(name: "La Dague", damage: 6.66f, attackRate: 1.5f));
		itemTable.Add(4, new Weapon(name: "Marteau du seigneur", damage: 50.0f, attackRate: 1.0f, weaponTypeID: 2));
		itemTable.Add(5, new Weapon(name: "Arc du seigneur", attackRate: 0.5f, weaponTypeID: 3, projectileID: 1));
		itemTable.Add(6, new Armor(name: "Armure du Seigneur", slot: ArmorSlot.TORSO, setID: 1));
		itemTable.Add(7, new Armor(name: "Casque du Seigneur", slot: ArmorSlot.HEAD, setID: 1));
		itemTable.Add(8, new Armor(name: "Bottes du Seigneur", slot: ArmorSlot.FEET, setID: 1));

		// Monsters
		temp = new uint[1];
		temp[0] = 1;
		monsterTable.Add(1, new Monster(name: "Zombie", behaviour: AIType.aggressive, modelPath: "Cylinder", items: temp));

		temp = new uint[5];
		temp[0] = 4;
		temp[1] = 5;
		temp[2] = 6;
		temp[3] = 7;
		temp[4] = 8;
		monsterTable.Add(2, new Monster(name: "Lord", behaviour: AIType.aggressive, items: temp));
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
