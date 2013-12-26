using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class DataTables
{
	static Dictionary <uint, Item> ItemTable = new Dictionary<uint, Item>();

	static Dictionary <uint, WeaponType> WeaponTypeTable = new Dictionary<uint, WeaponType>();

	static Dictionary <uint, Monster> MonsterTable = new Dictionary<uint, Monster>();
	//static Dictionary <uint, Projectile> ProjectileTable = new Dictionary<uint, Projectile>();
	//static Dictionary <uint, Skill> SkillTable = new Dictionary<uint, Skill>();
	
	public enum AIType : uint { Defensive, Aggressive, Roaming };

	private static void clearTables()
	{
		ItemTable.Clear();
		WeaponTypeTable.Clear();
		MonsterTable.Clear();
	}

	public static void updateTables()
	{
		clearTables();

		// Items
		ItemTable.Add(1, new Weapon("Epee des mile phote d'ortograff", 10, 1));
		ItemTable.Add(2, new Armor("Armure du test ultime"));

		// Weapon types
		WeaponTypeTable.Add(1, new WeaponType("Lame courte"));

		// Monsters
		MonsterTable.Add(1, new Monster("Monstre"));
	}

	public static Item getWeapon(uint key)
	{
		if(ItemTable.ContainsKey(key))
		{
			return ItemTable[key];
		}
		return null;
	}

	public static WeaponType getWeaponType(uint key)
	{
		if(WeaponTypeTable.ContainsKey(key))
		{
			return WeaponTypeTable[key];
		}
		return null;
	}

	public static Monster getMonster(uint key)
	{
		if(MonsterTable.ContainsKey(key))
		{
			return MonsterTable[key];
		}
		return null;
	}
}
