using UnityEngine;
using System.Collections;

public class Monster
{
	string name;		// Name of the monster
	AIType behaviour;	// Defines how the monster should act and react to enemies

	string modelPath;	// Model of the monster
	float scale;		// Model and hitbox scale of the monster

	uint[] items;		// Indexes of entries from the weapon table that this monster is carrying

	// TODO: Stats

	public Monster(string name)
	{
		this.name = name;
		this.behaviour = AIType.defensive;
	}

	public string getName() { return this.name; }
	public string getModelPath() { return this.modelPath; }
	public float getScale() { return this.scale; }
	public uint[] getItems() { return this.items; }
}