using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : WikiEntry
{
	private AIType behaviour;	// Defines how the monster should act and react to enemies
	private List<uint> items;	// Indexes of entries from the weapon table that this monster is carrying
	private List<uint> buffs;	// Indexes of entries from the buff table that this monster has by default

	public Monster(Metadata metadata,
		AIType behaviour = AIType.defensive,
		uint[] items = null,
		uint[] buffs = null)
		: base(metadata)
	{
		this.behaviour = behaviour;

		if (items == null)
			this.items = new List<uint>();
		else
			this.items = new List<uint>(items);

		if (buffs == null)
			this.buffs = new List<uint>();
		else
			this.buffs = new List<uint>(buffs);
	}

	public AIType GetBehaviour() { return this.behaviour; }
	public List<uint> GetItems() { return this.items; }
	public List<uint> GetBuffs() { return this.buffs; }

	public override void DrawDataWindow(float width, float height)
	{
		base.DrawDataWindow(width, height);
	}

	public override void DrawWikiPage(float width, float height)
	{
		base.DrawWikiPage(width, height);
	}
}