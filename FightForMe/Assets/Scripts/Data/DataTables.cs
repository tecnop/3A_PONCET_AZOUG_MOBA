using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class DataTables
{ // NOTE: All table IDs start at 1, 0 is used as a null value

	// Entities
	static Dictionary<uint, Monster> MonsterTable = new Dictionary<uint, Monster>();
	static Dictionary<uint, Projectile> ProjectileTable = new Dictionary<uint, Projectile>();

	// Items
	static Dictionary<uint, Item> ItemTable = new Dictionary<uint, Item>();
	static Dictionary<uint, WeaponType> WeaponTypeTable = new Dictionary<uint, WeaponType>();
	static Dictionary<uint, ArmorSet> ArmorSetTable = new Dictionary<uint, ArmorSet>();

	// Skill Tree
	static Dictionary<uint, Skill> SkillTable = new Dictionary<uint, Skill>();

	private static void clearTables()
	{
		MonsterTable.Clear();
		ProjectileTable.Clear();

		ItemTable.Clear();
		WeaponTypeTable.Clear();
		ArmorSetTable.Clear();

		SkillTable.Clear();
	}

	public static void updateTables()
	{
		clearTables();

		// NOTE: To account for dependencies, the tables should be initialized in the following order:
		// 1 - Skill tree
		// 2 - Projectiles
		// 3 - Weapon types, armor sets
		// 4 - Items
		// 5 - Monsters
		// Technically it doesn't matter because we only use IDs (for now), but for the sake of consistency this order should be used

		// Items
		ItemTable.Add(1, new Weapon(name: "Epee des mile phote d'ortograff", damage: 10.0f, attackRate: 1.0f));
		ItemTable.Add(2, new Armor(name: "Armure du test ultime"));
		ItemTable.Add(3, new Weapon(name: "La Dague", damage: 6.66f, attackRate: 1.5f));

		// Weapon types
		WeaponTypeTable.Add(1, new WeaponType(name: "Lame courte"));

		// Monsters
		uint[] temp = new uint[1];
		temp[0] = 1;
		MonsterTable.Add(1, new Monster(name: "Zombie", behaviour: AIType.aggressive, items:temp));
	}

	public static Item getItem(uint key)
	{
		if (ItemTable.ContainsKey(key))
		{
			return ItemTable[key];
		}
		return null;
	}

	public static WeaponType getWeaponType(uint key)
	{
		if (WeaponTypeTable.ContainsKey(key))
		{
			return WeaponTypeTable[key];
		}
		return null;
	}

	public static Monster getMonster(uint key)
	{
		if (MonsterTable.ContainsKey(key))
		{
			return MonsterTable[key];
		}
		return null;
	}
}
