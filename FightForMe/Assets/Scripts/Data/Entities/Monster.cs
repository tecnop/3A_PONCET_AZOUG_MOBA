using UnityEngine;
using System.Collections;

public class Monster
{
	private string name;		// Name of the monster
	private AIType behaviour;	// Defines how the monster should act and react to enemies

	private string modelPath;	// Model of the monster
	private float scale;		// Model and hitbox scale of the monster

	private ArrayList items;	// Indexes of entries from the weapon table that this monster is carrying
	private ArrayList buffs;	// Indexes of entries from the buff table that this monster has by default

	public Monster(string name = null,
		AIType behaviour = AIType.defensive,
		string modelPath = null,
		float scale = 1.0f,
		uint[] items = null,
		uint[] buffs = null)
	{
		if (name == null)
			this.name = "Monster";
		else
			this.name = name;

		this.behaviour = behaviour;

		this.modelPath = modelPath;

		this.scale = Mathf.Clamp(scale, 0.1f, 10.0f);

		if (items == null)
			this.items = new ArrayList();
		else
			this.items = new ArrayList(items);

		if (buffs == null)
			this.buffs = new ArrayList();
		else
			this.buffs = new ArrayList(buffs);
	}

	public string getName() { return this.name; }
	public AIType getBehaviour() { return this.behaviour; }
	public string getModelPath() { return this.modelPath; }
	public float getScale() { return this.scale; }
	public ArrayList getItems() { return this.items; }
}