using UnityEngine;
using System.Collections;

public class Monster : WikiEntry
{
	private AIType behaviour;	// Defines how the monster should act and react to enemies
	private ArrayList items;	// Indexes of entries from the weapon table that this monster is carrying
	private ArrayList buffs;	// Indexes of entries from the buff table that this monster has by default

	public Monster(string name = "Monster",
		string description = null,
		string description2 = null,
		string modelPath = null,
		float scale = 1.0f,
		string iconPath = null,
		Quality quality = Quality.COMMON,
		AIType behaviour = AIType.defensive,
		uint[] items = null,
		uint[] buffs = null)
		: base(name, description, description2, new GameModel(modelPath, scale), iconPath, quality)
	{
		this.behaviour = behaviour;

		if (items == null)
			this.items = new ArrayList();
		else
			this.items = new ArrayList(items);

		if (buffs == null)
			this.buffs = new ArrayList();
		else
			this.buffs = new ArrayList(buffs);
	}

	public AIType GetBehaviour() { return this.behaviour; }
	public ArrayList GetItems() { return this.items; }
	public ArrayList GetBuffs() { return this.buffs; }
}