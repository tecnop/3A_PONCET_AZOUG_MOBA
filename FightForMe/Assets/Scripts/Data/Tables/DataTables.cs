using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class DataTables
{ // NOTE: All table IDs start at 1, 0 is used as a null value

	// Parsing params.
	static string configPath = "../Configs/main.cfg";
	static string currentLang = "fr";

	static Exception parsingError;

	static string configContents;

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

	private static void ClearTables()
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

		// Reset the error
		parsingError = null;
	}

	public static void LoadModels(GameObject[] models)
	{
		modelTable.Clear(); // Temporary

		foreach (GameObject obj in models)
		{
			modelTable.Add(obj.name, obj);
		}
	}

	public static void FillTables()
	{
		ClearTables();

		if (GameData.secure)
		{
			ParseDefaultTables();
		}
		else
		{
			if (GameData.isServer)
			{
				try
				{
					ParseTablesFromFile();
				}
				catch (Exception e)
				{
					configContents = null;
					parsingError = e;
				}
			}
			// Clients will wait for the server to send it to them
			return;
		}
	}

	private static void ParseDefaultTables()
	{
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
		/*spellTable.Add(4, new SpellFireball());
		spellTable.Add(5, new SpellFireExplosion());
		spellTable.Add(6, new SpellBurn());
		spellTable.Add(7, new SpellGrenade());
		spellTable.Add(8, new SpellExplosion());
		spellTable.Add(9, new SpellKnockback());*/
		spellTable.Add(4, new SpellProjShot(new Metadata("Boule de feu"), 2, 1, costType: SpellCostType.MANA, spellCost: 50.0f));
		spellTable.Add(5, new SpellAreaOfEffect(new Metadata("Explosion de feu"), 4.0f, 6, particles: true));
		spellTable.Add(6, new SpellImpact(new Metadata("Brûlure"), 10.0f, 3, 5.0f));
		spellTable.Add(7, new SpellProjShot(new Metadata("Grenade"), 5, 1, doThrow: true, costType: SpellCostType.MANA, spellCost: 150.0f));
		spellTable.Add(8, new SpellAreaOfEffect(new Metadata("Explosion"), 6.0f, 9, particles: true));
		spellTable.Add(9, new SpellImpact(new Metadata("Impact"), 10.0f, 0, 0.0f, 10.0f, 1.0f));
		spellTable.Add(10, new SpellProjShot(new Metadata("Tir multiple"), 0, 5, impactSpellOverride: 3, costType: SpellCostType.MANA, spellCost: 30.0f));
		spellTable.Add(11, new SpellDash(new Metadata("Charge"), 30.0f, 1.0f, true, impactSpell: 12, costType: SpellCostType.HEALTH, spellCost: 40.0f));
		spellTable.Add(12, new SpellImpact(new Metadata("Impact de charge"), 15, 0, 0.0f, 30.0f, 2.0f));
		spellTable.Add(13, new SpellToggleBuff(new Metadata("Régénération"), 5, costType: SpellCostType.PCTHEALTH, spellCost: 0.1f));

		// Effects
		effectTable.Add(1, new Effect(isPositive: true, flatHP: 2000, stats: new Stats(50, 50, 50)));
		effectTable.Add(2, new Effect(isPositive: true, unlockedAbility: 1));
		effectTable.Add(3, new Effect(isPositive: true, stats: new Stats(strength: 10)));
		effectTable.Add(4, new Effect(isPositive: true, stats: new Stats(agility: 10)));
		effectTable.Add(5, new Effect(isPositive: true, stats: new Stats(intelligence: 10)));
		effectTable.Add(6, new Effect(isPositive: true, stats: new Stats(strength: 30)));
		effectTable.Add(7, new Effect(isPositive: true, stats: new Stats(agility: 30)));
		effectTable.Add(8, new Effect(isPositive: true, stats: new Stats(intelligence: 30)));
		effectTable.Add(9, new Effect(isPositive: true, flatMS: 350.0f));
		effectTable.Add(10, new Effect(isPositive: true, flatMS: 350.0f));
		effectTable.Add(11, new Effect(isPositive: true, stats: new Stats(200, 200, 200), bonusDamage: 1000.0f, bonusProjDamage: 1000.0f));
		effectTable.Add(12, new Effect(isPositive: false, pctHPRegen: -0.05f));
		effectTable.Add(13, new Effect(isPositive: true, unlockedAbility: 4));
		// ============= HARD-CODED REFERENCE =============
		effectTable.Add(14, new Effect(description: "Attaque", isPositive: true, unlockedAbility: 1));
		// ================================================
		effectTable.Add(15, new Effect(isPositive: true, unlockedAbility: 7));
		// ============= HARD-CODED REFERENCE =============
		effectTable.Add(16, new Effect(description: "Relique", isPositive: true, misc: MiscEffect.CARRYING_TROPHY));
		// ================================================
		effectTable.Add(17, new Effect(isPositive: true, unlockedAbility: 10));
		effectTable.Add(18, new Effect(isPositive: true, unlockedAbility: 11));
		effectTable.Add(19, new Effect(isPositive: true, unlockedAbility: 13));
		effectTable.Add(20, new Effect(isPositive: true, pctHPRegen: 0.05f));
		effectTable.Add(21, new Effect(isPositive: true, misc: MiscEffect.INVULNERABLE));
		effectTable.Add(22, new Effect(isPositive: false, bonusDamage: -0.75f, bonusProjDamage: -0.75f));
		effectTable.Add(23, new Effect(isPositive: true, flatHP: 1500, stats: new Stats(50, 50, 50)));

		// Buffs
		buffTable.Add(1, new Buff(metadata: new Metadata(name: "Seigneur"), effects: new uint[] { 1 }));

		buffTable.Add(2, new Buff(metadata: new Metadata(name: "Testeur"), effects: new uint[] { 11 }));

		buffTable.Add(3, new Buff(metadata: new Metadata(name: "Brûlure"), effects: new uint[] { 12 }));

		buffTable.Add(4, new Buff(metadata: new Metadata(name: "Relique"), effects: new uint[] { 16 }));

		buffTable.Add(5, new Buff(metadata: new Metadata(name: "HP+"), effects: new uint[] { 20, 22 }));

		buffTable.Add(6, new Buff(metadata: new Metadata(name: "Banlieue"), effects: new uint[] { 9, 3 }));

		// ============= HARD-CODED REFERENCE =============
		buffTable.Add(7, new Buff(metadata: new Metadata(name: "Invincible"), effects: new uint[] { 21 }));
		// ================================================

		buffTable.Add(8, new Buff(metadata: new Metadata(name: "Hasnor"), effects: new uint[] { 23 }));

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
		skillTable.Add(14, new Skill(metadata: new Metadata(name: "Sort: Régénération"), color: SkillColor.R, effect: 19, neighbours: new uint[] { 13 }));

		// Projectiles
		projectileTable.Add(1, new Projectile(metadata: new Metadata(name: "Flèche du seigneur"), damage: 30.0f, speed: 50.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(2, new Projectile(metadata: new Metadata(name: "Boule de feu"), damage: 15.0f, speed: 10.0f, hitboxSize: new Vector3(0.5f, 0.5f, 0.5f), impactAbility: 5));
		projectileTable.Add(3, new Projectile(metadata: new Metadata(name: "Balle"), damage: 50.0f, speed: 150.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(4, new Projectile(metadata: new Metadata(name: "Flèche"), damage: 10.0f, speed: 50.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));
		projectileTable.Add(5, new Projectile(metadata: new Metadata(name: "Grenade"), damage: 15.0f, speed: 10.0f, hitboxSize: new Vector3(0.5f, 0.5f, 0.5f), impactAbility: 8, trajectory: ProjectileTrajectory.Throw));
		projectileTable.Add(6, new Projectile(metadata: new Metadata(name: "Couteau de lancer"), damage: 7.5f, speed: 75.0f, hitboxSize: new Vector3(0.25f, 0.25f, 1.0f)));

		// Weapon types
		weaponTypeTable.Add(1, new WeaponType(metadata: new Metadata(name: "Epée courte"), isTwoHanded: false));
		weaponTypeTable.Add(2, new WeaponType(metadata: new Metadata(name: "Marteau")));
		weaponTypeTable.Add(3, new WeaponType(metadata: new Metadata(name: "Arc"), isRanged: true));
		weaponTypeTable.Add(4, new WeaponType(metadata: new Metadata(name: "Arme de lancer"), isRanged: true));
		weaponTypeTable.Add(5, new WeaponType(metadata: new Metadata(name: "Epée longue")));
		weaponTypeTable.Add(6, new WeaponType(metadata: new Metadata(name: "Debug"), isTwoHanded: false));

		// Armor sets
		armorSetTable.Add(1, new ArmorSet(metadata: new Metadata(name: "Panoplie du Seigneur"), buffID: 1));
		armorSetTable.Add(2, new ArmorSet(metadata: new Metadata(name: "Le Beta testeur"), buffID: 2));
		armorSetTable.Add(3, new ArmorSet(metadata: new Metadata(name: "Le jeune de banlieue"), buffID: 6));

		// Items
		itemTable.Add(1, new Weapon(metadata: new Metadata(name: "Epee des mile phote d'ortograff"), damage: 20.0f, attackRate: 1.0f, weaponTypeID: 5));
		itemTable.Add(2, new Armor(metadata: new Metadata(name: "Armure du test ultime"), slot: ArmorSlot.BODY, setID: 2));
		itemTable.Add(3, new Weapon(metadata: new Metadata(name: "La Dague"), damage: 6.66f, attackRate: 1.5f, weaponTypeID: 1));
		itemTable.Add(4, new Weapon(metadata: new Metadata(name: "Marteau du Seigneur"), damage: 50.0f, attackRate: 1.0f, weaponTypeID: 2));
		itemTable.Add(5, new Weapon(metadata: new Metadata(name: "Arc du Seigneur"), attackRate: 0.5f, weaponTypeID: 3, projectileID: 1));
		itemTable.Add(6, new Armor(metadata: new Metadata(name: "Armure du Seigneur"), slot: ArmorSlot.BODY, setID: 1));
		itemTable.Add(7, new Armor(metadata: new Metadata(name: "Casque du Seigneur"), slot: ArmorSlot.HEAD, setID: 1));
		itemTable.Add(8, new Armor(metadata: new Metadata(name: "Bottes du Seigneur"), slot: ArmorSlot.FEET, setID: 1));
		itemTable.Add(9, new Weapon(metadata: new Metadata(name: "Le fouetteur d'Yggtralala"), damage: 15.0f, attackRate: 1.0f));
		itemTable.Add(10, new Armor(metadata: new Metadata(name: "Casque du super debug"), slot: ArmorSlot.HEAD, setID: 2));
		itemTable.Add(11, new Armor(metadata: new Metadata(name: "Gants de l'incroyable fix"), slot: ArmorSlot.HANDS, setID: 2));
		itemTable.Add(12, new Armor(metadata: new Metadata(name: "Bottes de l'interminable alpha"), slot: ArmorSlot.FEET, setID: 2));
		itemTable.Add(13, new Weapon(metadata: new Metadata(name: "Croc de la Téci"), damage: 6.0f, attackRate: 2.0f, weaponTypeID: 1));
		itemTable.Add(14, new Weapon(metadata: new Metadata(name: "La quat'cinq"), attackRate: 0.3f, weaponTypeID: 3, projectileID: 3));
		itemTable.Add(15, new Weapon(metadata: new Metadata(name: "Arc biodégradable"), attackRate: 1.0f, weaponTypeID: 3, projectileID: 4));
		itemTable.Add(16, new Weapon(metadata: new Metadata(name: "Pierre à XP"), recyclingXP: 1000, weaponTypeID: 6));

		itemTable.Add(17, new Weapon(metadata: new Metadata(name: "Le Totem"), damage: 35.0f, attackRate: 1.2f, weaponTypeID: 2));
		itemTable.Add(18, new Weapon(metadata: new Metadata(name: "Les coordanites"), weaponTypeID: 4, damage: 25.0f, attackRate: 1.5f, projectileID: 6));

		itemTable.Add(19, new Armor(metadata: new Metadata(name: "Sweat et jeans troués"), slot: ArmorSlot.BODY, setID: 3, stats: new Stats(5, 10, 0)));
		itemTable.Add(20, new Armor(metadata: new Metadata(name: "Casquette retournée"), slot: ArmorSlot.HEAD, setID: 3, stats: new Stats(5, 10, 0)));
		itemTable.Add(21, new Armor(metadata: new Metadata(name: "Les niques"), slot: ArmorSlot.FEET, setID: 3, stats: new Stats(5, 10, 0)));

		// Monsters

		// ============= HARD-CODED REFERENCE =============
		monsterTable.Add(1, new Monster(metadata: new Metadata(name: "Lord", quality: Quality.UNIQUE), behaviour: AIType.defensive, items: new uint[] { 4, 5, 6, 7, 8 }));
		// ================================================

		monsterTable.Add(2, new Monster(metadata: new Metadata(name: "Zombie", modelPath: "Cylinder"), behaviour: AIType.defensive, items: new uint[] { 9 }));
		monsterTable.Add(3, new Monster(metadata: new Metadata(name: "Ratus", scale: 0.5f), behaviour: AIType.defensive, items: new uint[] { 13 }));
		monsterTable.Add(4, new Monster(metadata: new Metadata(name: "Archet"), behaviour: AIType.defensive, items: new uint[] { 15 }));
		monsterTable.Add(5, new Monster(metadata: new Metadata(name: "Snaille'p"), behaviour: AIType.defensive, items: new uint[] { 14 }));

		// Debug Monsters
		monsterTable.Add(6, new Monster(metadata: new Metadata(name: "Debug1"), behaviour: AIType.defensive, items: new uint[] { 2, 10 }));
		monsterTable.Add(7, new Monster(metadata: new Metadata(name: "Debug2"), behaviour: AIType.defensive, items: new uint[] { 11, 12 }));

		// ============= HARD-CODED REFERENCE =============
		monsterTable.Add(8, new Monster(metadata: new Metadata(name: "Hasnor", scale: 2.0f, quality: Quality.UNIQUE), behaviour: AIType.aggressive, buffs: new uint[] { 8 }));
		// ================================================

		monsterTable.Add(9, new Monster(metadata: new Metadata(name: "Jean Paul"), behaviour: AIType.defensive, items: new uint[] { 17 }));
		monsterTable.Add(10, new Monster(metadata: new Metadata(name: "Jacky"), behaviour: AIType.aggressive, items: new uint[] { 19, 20, 21 }));

		monsterTable.Add(11, new Monster(metadata: new Metadata(name: "Le veau doux"), behaviour: AIType.aggressive, items: new uint[] { 18 }));

		monsterTable.Add(12, new Monster(metadata: new Metadata(name: "Un monstre")));
	}

	private static void ParseTablesFromFile()
	{ // Parse the tables from a local file
		try
		{
			StreamReader config = File.OpenText(configPath);
			configContents = config.ReadToEnd();
			config.Close();
		}
		catch
		{
			throw new Exception("Le fichier de configuration n'a pas pu être ouvert, assurez-vous qu'il se trouve à "+configPath);
		}

		try
		{
			ParseConfigString();
		}
		catch (Exception e)
		{
			throw e;
		}
	}

	public static void SetConfigString(string configString)
	{ // Parse the tables we received from an external source (most likely the server we connected to)
		configContents = configString;

		ClearTables();

		try
		{
			ParseConfigString();
		}
		catch (Exception e)
		{
			throw e;
		}
	}

	public static string GetConfigString()
	{
		return configContents;
	}

	private static void ParseConfigString()
	{ // Changing a few things to allow transfer of the file's contents to clients
		FJsonParser parser = FJsonParser.Instance();
		//parser.parseFile(configPath);
		try
		{
			parser.parseString(configContents);
		}
		catch
		{
			throw new Exception("Erreur lors de la lecture du fichier de configuration");
		}

		if (parser.getResults().Count == 0)
		{ // We didn't get anything, maybe the file is invalid; be safe and drop it, don't send it to clients
			configContents = null;
			throw new Exception("Erreur inconnue lors de la lecture du fichier de configuration");
		}

		try
		{
			foreach (Clazz ns in parser.getResults())
			{
				if (ns.getName() == "monster")
				{
					pushMonster(ns.getFields());
				}
				else if (ns.getName() == "effect")
				{
					pushEffect(ns.getFields());
				}
				else if (ns.getName() == "weapon")
				{
					pushWeapon(ns.getFields());
				}
				else if (ns.getName() == "armor")
				{
					pushArmor(ns.getFields());
				}
				else if (ns.getName() == "projectile")
				{
					pushProjectile(ns.getFields());
				}
				else
				{
					Debug.LogWarning("Nothing found for parsed class '" + ns.getName() + "'");
				}
			}
		}
		catch (Exception e)
		{
			throw e;
			//throw new Exception("Incohérence dans le fichier de configuration");
		}
	}

	private static void pushMonster(Dictionary<string, string> fields)
	{
		Debug.Log("Clazz : 'Monster'");
		foreach (KeyValuePair<string, string> k in fields)
		{
			Debug.Log("Key : '" + k.Key + "' Value : '" + k.Value + "'");

		}

		/* expected fields */
		uint id = 0;
		Metadata metadata = null;
		AIType behaviour = AIType.defensive;
		uint[] items = null;
		uint[] buffs = null;

		id = uint.Parse(fields["id"]);

		if (fields.ContainsKey("Metadata"))
		{
			metadata = getMetadata(fields["Metadata"]);
		}
		if (fields.ContainsKey("behaviour"))
		{
			behaviour = (AIType)int.Parse(fields["behaviour"]);
		}
		if (fields.ContainsKey("items"))
		{
			items = stringToUintArray(fields["items"]);
		}
		if (fields.ContainsKey("buffs"))
		{
			buffs = stringToUintArray(fields["buffs"]);
		}

		try
		{
			monsterTable.Add(id, new Monster(metadata, behaviour, items, buffs));
		}
		catch (Exception e)
		{
			throw new Exception("Erreur lors de la lecture des données du monstre " + (metadata.GetName() ?? id.ToString()) + ":\n" + e.Message);
		}
	}

	private static void pushWeapon(Dictionary<string, string> fields)
	{
		Debug.Log("Clazz : 'Weapon'");
		foreach (KeyValuePair<string, string> k in fields)
		{
			Debug.Log("Key : '" + k.Key + "' Value : '" + k.Value + "'");

		}

		/* expected fields */
		uint id = 0;
		Metadata metadata = null;

		uint recyclingXP = 100;
		uint buffID = 0;
		uint weaponTypeID = 0;
		float damage = 0.0f;
		float attackRate = 1.0f;
		uint projectileID = 0;
		string effectPath = null;
		string attackSoundPath = null;



		id = uint.Parse(fields["id"]);

		if (fields.ContainsKey("Metadata"))
		{
			metadata = getMetadata(fields["Metadata"]);
		}

		if (fields.ContainsKey("buffID"))
		{
			buffID = uint.Parse(fields["buffID"]);
		}
		if (fields.ContainsKey("weaponTypeID"))
		{
			weaponTypeID = uint.Parse(fields["weaponTypeID"]);
		}
		if (fields.ContainsKey("recyclingXP"))
		{
			recyclingXP = uint.Parse(fields["recyclingXP"]);
		}

		if (fields.ContainsKey("damage"))
		{
			damage = float.Parse(fields["damage"]);
		}
		if (fields.ContainsKey("attackRate"))
		{
			attackRate = float.Parse(fields["attackRate"]);
		}

		if (fields.ContainsKey("projectileID"))
		{
			projectileID = uint.Parse(fields["projectileID"]);
		}
		/* Coming soon !
		fields.TryGetValue ("effectPath", out effectPath);
		fields.TryGetValue ("attackSoundPath", out attackSoundPath);
		*/

		try
		{
			itemTable.Add(id, new Weapon(metadata, recyclingXP, buffID, weaponTypeID, damage, attackRate, projectileID, effectPath, attackSoundPath));
		}
		catch (Exception e)
		{
			throw new Exception("Erreur lors de la lecture des données de l'arme " + (metadata.GetName() ?? id.ToString()) + ":\n" + e.Message);
		}
	}

	private static void pushArmor(Dictionary<string, string> fields)
	{
		Debug.Log("Clazz : 'Armor'");
		foreach (KeyValuePair<string, string> k in fields)
		{
			Debug.Log("Key : '" + k.Key + "' Value : '" + k.Value + "'");

		}

		/* expected fields */
		uint id = 0;
		Metadata metadata = null;
		uint recyclingXP = 100;
		uint buffID = 0;
		ArmorSlot slot = ArmorSlot.BODY;
		uint setID = 0;
		Stats stats = null;

		id = uint.Parse(fields["id"]);

		if (fields.ContainsKey("Metadata"))
		{
			metadata = getMetadata(fields["Metadata"]);
		}

		if (fields.ContainsKey("recyclingXP"))
		{
			recyclingXP = uint.Parse(fields["recyclingXP"]);
		}

		if (fields.ContainsKey("buffID"))
		{
			buffID = uint.Parse(fields["buffID"]);
		}

		if (fields.ContainsKey("slot"))
		{
			slot = (ArmorSlot)int.Parse(fields["slot"]);
		}

		if (fields.ContainsKey("setID"))
		{
			setID = uint.Parse(fields["setID"]);
		}

		if (fields.ContainsKey("stats"))
		{
			stats = stringToStats(fields["stats"]);
		}

		try
		{
			itemTable.Add(id, new Armor(metadata, recyclingXP, buffID, slot, setID, stats));
		}
		catch (Exception e)
		{
			throw new Exception("Erreur lors de la lecture des données de l'armure " + (metadata.GetName() ?? id.ToString()) + ":\n" + e.Message);
		}
	}

	private static void pushProjectile(Dictionary<string, string> fields)
	{
		Debug.Log("Clazz : 'Armor'");
		foreach (KeyValuePair<string, string> k in fields)
		{
			Debug.Log("Key : '" + k.Key + "' Value : '" + k.Value + "'");

		}

		/* expected fields */
		uint id = 0;
		Metadata metadata = null;

		string effectPath = null;
		string impactEffectPath = null;
		float damage = 0.0f;
		float speed = 1.0f;
		uint impactAbility = 0;
		Vector3 hitboxSize = new Vector3();
		float range = 0;
		float lifeTime = 0;

		// Coming soon !
		ProjectileTrajectory trajectory = ProjectileTrajectory.Straight;
		ProjectileCollisionType collision = ProjectileCollisionType.Everything;

		id = uint.Parse(fields["id"]);

		if (fields.ContainsKey("Metadata"))
		{
			metadata = getMetadata(fields["Metadata"]);
		}

		fields.TryGetValue("effectPath", out effectPath);
		fields.TryGetValue("impactEffectPath", out impactEffectPath);

		if (fields.ContainsKey("damage"))
		{
			damage = float.Parse(fields["damage"]);
		}

		if (fields.ContainsKey("speed"))
		{
			speed = float.Parse(fields["speed"]);
		}

		if (fields.ContainsKey("impactAbility"))
		{
			impactAbility = uint.Parse(fields["impactAbility"]);
		}

		if (fields.ContainsKey("hitboxSize"))
		{
			hitboxSize = stringToVector3(fields["hitboxSize"]);
		}

		if (fields.ContainsKey("range"))
		{
			range = float.Parse(fields["range"]);
		}

		if (fields.ContainsKey("lifeTime"))
		{
			lifeTime = uint.Parse(fields["lifeTime"]);
		}

		try
		{
			projectileTable.Add(id, new Projectile(metadata, effectPath, impactEffectPath, damage, speed, impactAbility, hitboxSize, range, lifeTime, trajectory, collision));
		}
		catch (Exception e)
		{
			throw new Exception("Erreur lors de la lecture des données du projectile " + (metadata.GetName() ?? id.ToString()) + ":\n" + e.Message);
		}
	}

	private static void pushEffect(Dictionary<string, string> fields)
	{
		Debug.Log("Clazz : 'Effect'");
		foreach (KeyValuePair<string, string> k in fields)
		{
			Debug.Log("Key : '" + k.Key + "' Value : '" + k.Value + "'");

		}
		/* expected fields */
		uint id = 0;
		string description = ""; // Helper for the effect, not the metadata description
		bool isPositive = true;
		float flatHP = 0.0f;
		float pctHP = 0.0f;
		float flatMP = 0.0f;
		float pctMP = 0.0f;
		float flatHPRegen = 0.0f;
		float pctHPRegen = 0.0f;
		float flatMPRegen = 0.0f;
		float pctMPRegen = 0.0f;
		float flatMS = 0.0f;
		float pctMS = 0.0f;
		float bonusDamage = 0.0f;
		float bonusAtkSpd = 0.0f;
		float bonusProjDamage = 0.0f;
		Stats stats = null;
		uint unlockedAbility = 0;
		MiscEffect misc = MiscEffect.NONE;
		float miscParm = 0.0f;


		id = uint.Parse(fields["id"]);

		/* Useless
		if(fields.ContainsKey("Metadata")){
			metadata = getMetadata(fields["Metadata"]);
		}
		*/

		fields.TryGetValue("description", out description);

		if (fields.ContainsKey("isPositive"))
		{
			isPositive = int.Parse(fields["isPositive"]) == 0 ? false : true;
		}
		if (fields.ContainsKey("flatHP"))
		{
			flatHP = float.Parse(fields["flatHP"]);
		}

		if (fields.ContainsKey("pctHP"))
		{
			pctHP = float.Parse(fields["pctHP"]);
		}

		if (fields.ContainsKey("flatMP"))
		{
			flatMP = float.Parse(fields["flatMP"]);
		}

		if (fields.ContainsKey("pctMP"))
		{
			pctMP = float.Parse(fields["pctMP"]);
		}

		if (fields.ContainsKey("flatHPRegen"))
		{
			flatHPRegen = float.Parse(fields["flatHPRegen"]);
		}

		if (fields.ContainsKey("pctHPRegen"))
		{
			pctHPRegen = float.Parse(fields["pctHPRegen"]);
		}

		if (fields.ContainsKey("flatMPRegen"))
		{
			flatMPRegen = float.Parse(fields["flatMPRegen"]);
		}

		if (fields.ContainsKey("pctMPRegen"))
		{
			pctMPRegen = float.Parse(fields["pctMPRegen"]);
		}

		if (fields.ContainsKey("flatMS"))
		{
			flatMS = float.Parse(fields["flatMS"]);
		}

		if (fields.ContainsKey("pctMS"))
		{
			pctMS = float.Parse(fields["pctMS"]);
		}

		if (fields.ContainsKey("bonusDamage"))
		{
			bonusDamage = float.Parse(fields["bonusDamage"]);
		}

		if (fields.ContainsKey("bonusAtkSpd"))
		{
			bonusAtkSpd = float.Parse(fields["bonusAtkSpd"]);
		}

		if (fields.ContainsKey("bonusProjDamage"))
		{
			bonusProjDamage = float.Parse(fields["bonusProjDamage"]);
		}

		if (fields.ContainsKey("stats"))
		{
			stats = stringToStats(fields["stats"]);
		}
		/* // Coming soon !
		if(fields.ContainsKey("unlockedAbility")){ 

		}
		if(fields.ContainsKey("misc")){

		}
		if(fields.ContainsKey("miscParm")){
			miscParm = ...
		}
		*/
		try
		{
			effectTable.Add(id, new Effect(isPositive, description, flatHP, pctHP, flatMP, pctMP, flatHPRegen, pctHPRegen, flatMPRegen, pctMPRegen, flatMS, pctMS, bonusDamage, bonusAtkSpd, bonusProjDamage, stats, unlockedAbility, misc, miscParm));
		}
		catch (Exception e)
		{
			throw new Exception("Erreur lors de la lecture des données de l'effet " + id + ":\n" + e.Message);
		}
	}

	private static Metadata getMetadata(string pattern)
	{
		/* Expected fields */
		string name = null;
		string description = null;
		string description2 = null;
		string modelPath = null;
		float scale = 1.0f;
		string iconPath = null;
		Quality quality = Quality.COMMON;



		FJsonParser parser = FJsonParser.Instance();
		parser.parseString(pattern);
		Dictionary<string, string> fields = null;
		Clazz defaultLang = null;
		foreach (Clazz c in parser.getResults())
		{
			if (null == defaultLang)
				defaultLang = c;
			if (c.getName() == currentLang)
				fields = c.getFields();
		}
		if (null == fields)
			fields = defaultLang.getFields();

		fields.TryGetValue("name", out name);
		fields.TryGetValue("description", out description);
		fields.TryGetValue("description2", out description2);
		fields.TryGetValue("modelPath", out modelPath);
		fields.TryGetValue("iconPath", out iconPath);

		if (fields.ContainsKey("scale"))
		{
			scale = float.Parse(fields["scale"]);
		}

		if (fields.ContainsKey("quality"))
		{
			quality = (Quality)int.Parse(fields["quality"]);
		}

		return new Metadata(name, description, description2, modelPath, scale, iconPath, quality);
	}

	private static uint[] stringToUintArray(string pattern)
	{
		string[] values = pattern.Split(',');
		uint[] res = new uint[values.Length];
		int i = 0;
		foreach (string s in values)
		{
			res[i] = uint.Parse(s);
			i++;
		}

		return res;

	}

	private static Stats stringToStats(string pattern)
	{
		string[] values = pattern.Split(',');
		if (values.Length != 3)
		{
			return null;
		}

		// Puis. , Endu. , Int.
		return new Stats(uint.Parse(values[0]), uint.Parse(values[1]), uint.Parse(values[2]));

	}

	private static Vector3 stringToVector3(string pattern)
	{
		string[] values = pattern.Split(',');
		if (values.Length != 3)
		{
			return new Vector3();
		}

		// Puis. , Endu. , Int.
		return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
	}

	public static Exception GetError()
	{
		return parsingError;
	}

	public static Item GetItem(uint key)
	{
		if (itemTable.ContainsKey(key))
		{
			return itemTable[key];
		}
		return null;
	}

	public static List<Item> GetItems()
	{
		return new List<Item>(itemTable.Values);
	}

	public static WeaponType GetWeaponType(uint key)
	{
		if (weaponTypeTable.ContainsKey(key))
		{
			return weaponTypeTable[key];
		}
		return null;
	}

	public static List<WeaponType> GetWeaponTypes()
	{
		return new List<WeaponType>(weaponTypeTable.Values);
	}

	public static Monster GetMonster(uint key)
	{
		if (monsterTable.ContainsKey(key))
		{
			return monsterTable[key];
		}
		return null;
	}

	public static List<Monster> GetMonsters()
	{
		return new List<Monster>(monsterTable.Values);
	}

	public static ArmorSet GetArmorSet(uint key)
	{
		if (armorSetTable.ContainsKey(key))
		{
			return armorSetTable[key];
		}
		return null;
	}

	public static List<ArmorSet> GetArmorSets()
	{
		return new List<ArmorSet>(armorSetTable.Values);
	}

	public static Projectile GetProjectile(uint key)
	{
		if (projectileTable.ContainsKey(key))
		{
			return projectileTable[key];
		}
		return null;
	}

	public static List<Projectile> GetProjectiles()
	{
		return new List<Projectile>(projectileTable.Values);
	}

	public static Skill GetSkill(uint key)
	{
		if (skillTable.ContainsKey(key))
		{
			return skillTable[key];
		}
		return null;
	}

	public static List<Skill> GetSkills()
	{
		return new List<Skill>(skillTable.Values);
	}

	public static uint GetSkillID(Skill skill)
	{
		int i = 0;
		List<uint> keys = new List<uint>(skillTable.Keys);
		while (i < keys.Count)
		{
			uint key = keys[i];
			if (skillTable[key] == skill)
			{
				return key;
			}
			i++;
		}
		return 0;
	}

	public static Spell GetSpell(uint key)
	{
		if (spellTable.ContainsKey(key))
		{
			return spellTable[key];
		}
		return null;
	}

	public static List<Spell> GetSpells()
	{
		return new List<Spell>(spellTable.Values);
	}

	public static Buff GetBuff(uint key)
	{
		if (buffTable.ContainsKey(key))
		{
			return buffTable[key];
		}
		return null;
	}

	public static List<Buff> GetBuffs()
	{
		return new List<Buff>(buffTable.Values);
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
