using UnityEngine;
using System.Collections;

public class Monster
{
	private string name;		// Name of the monster
	private AIType behaviour;	// Defines how the monster should act and react to enemies

	private string modelPath;	// Model of the monster
	private float scale;		// Model and hitbox scale of the monster

	private ArrayList items;	// Indexes of entries from the weapon table that this monster is carrying
	private ArrayList skills;	// Indexes of entries from the skill table that this monster has access to

	public Monster(string name)
	{
		this.name = name;
		this.behaviour = AIType.defensive;
	}

	public string getName() { return this.name; }
	public string getModelPath() { return this.modelPath; }
	public float getScale() { return this.scale; }
	public ArrayList getItems() { return this.items; }
}