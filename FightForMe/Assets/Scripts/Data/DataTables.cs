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

	// Buffs
	static Dictionary<uint, Effect> EffectTable = new Dictionary<uint, Effect>();
	static Dictionary<uint, Buff> BuffTable = new Dictionary<uint, Buff>();

	private static void clearTables()
	{
		MonsterTable.Clear();
		ProjectileTable.Clear();

		ItemTable.Clear();
		WeaponTypeTable.Clear();
		ArmorSetTable.Clear();

		SkillTable.Clear();

		EffectTable.Clear();
		BuffTable.Clear();
	}

	public static void updateTables()
	{
		clearTables();

		// NOTE: To account for dependencies, the tables should be initialized in the following order:
		// 1 - Status changes (AKA effects)
		// 2 - Buffs
		// 3 - Skill tree
		// 4 - Projectiles
		// 5 - Weapon types, armor sets
		// 6 - Items
		// 7 - Monsters
		// Technically it doesn't matter because we only use IDs (for now), but for the sake of consistency this order should be used

		uint[] temp;

		// Effects
		EffectTable.Add(1, new Effect(description: "Stats du seigneur", isPositive: true, flatHP: 2000, stats: new Stats(50, 50, 50)));

		// Buffs
		temp = new uint[1];
		temp[0] = 1;
		BuffTable.Add(1, new Buff(name: "Seigneur", effects: temp));

		// Skills

		// Projectiles
		ProjectileTable.Add(1, new Projectile(name: "Flèche du seigneur", damage: 30.0f, speed: 5.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		ProjectileTable.Add(2, new Projectile(name: "Boule de feu", damage: 15.0f, speed: 1.0f, hitboxSize: new Vector3(0.5f, 0.5f, 0.5f), impactRadius: 2.0f));

		// Weapon types
		WeaponTypeTable.Add(1, new WeaponType(name: "Epee courte"));
		WeaponTypeTable.Add(2, new WeaponType(name: "Marteau"));
		WeaponTypeTable.Add(3, new WeaponType(name: "Arc"));

		// Armor sets
		ArmorSetTable.Add(1, new ArmorSet(name: "Panoplie du Seigneur", buffID: 1));

		// Items
		ItemTable.Add(1, new Weapon(name: "Epee des mile phote d'ortograff", damage: 10.0f, attackRate: 1.0f));
		ItemTable.Add(2, new Armor(name: "Armure du test ultime"));
		ItemTable.Add(3, new Weapon(name: "La Dague", damage: 6.66f, attackRate: 1.5f));
		ItemTable.Add(4, new Weapon(name: "Marteau du seigneur", damage: 50.0f, attackRate: 1.0f, weaponTypeID: 2));
		ItemTable.Add(5, new Weapon(name: "Arc du seigneur", attackRate: 0.5f, weaponTypeID: 3, projectileID: 1));
		ItemTable.Add(6, new Armor(name: "Armure du Seigneur", slot: ArmorSlot.TORSO, setID: 1));
		ItemTable.Add(7, new Armor(name: "Casque du Seigneur", slot: ArmorSlot.HEAD, setID: 1));
		ItemTable.Add(8, new Armor(name: "Bottes du Seigneur", slot: ArmorSlot.FEET, setID: 1));

		// Monsters
		temp = new uint[1];
		temp[0] = 1;
		MonsterTable.Add(1, new Monster(name: "Zombie", behaviour: AIType.aggressive, items: temp));

		temp = new uint[5];
		temp[0] = 4;
		temp[1] = 5;
		temp[2] = 6;
		temp[3] = 7;
		temp[4] = 8;
		MonsterTable.Add(2, new Monster(name: "Lord", behaviour: AIType.aggressive, items: temp));
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

	public static ArmorSet getArmorSet(uint key)
	{
		if (ArmorSetTable.ContainsKey(key))
		{
			return ArmorSetTable[key];
		}
		return null;
	}

	public static Projectile getProjectile(uint key)
	{
		if (ProjectileTable.ContainsKey(key))
		{
			return ProjectileTable[key];
		}
		return null;
	}

	public static Skill getSkill(uint key)
	{
		if (SkillTable.ContainsKey(key))
		{
			return SkillTable[key];
		}
		return null;
	}

	public static Buff getBuff(uint key)
	{
		if (BuffTable.ContainsKey(key))
		{
			return BuffTable[key];
		}
		return null;
	}

	public static Effect getEffect(uint key)
	{
		if (EffectTable.ContainsKey(key))
		{
			return EffectTable[key];
		}
		return null;
	}
}
