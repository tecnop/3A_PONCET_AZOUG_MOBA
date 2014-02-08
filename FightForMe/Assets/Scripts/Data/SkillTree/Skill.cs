using UnityEngine;
using System.Collections;

public enum SkillColor
{
	R,	// Strength
	G,	// Agility
	B,	// Intelligence
	RG,	// Strength and agility
	RB,	// Strength and intelligence
	GB,	// Agility and intelligence
	W	// Neutral (starting skill only please!)
}

public class Skill
{
	private string name;			// Preferably single-word descriptive name
	private string description;		// Longer description
	private SkillColor color;		// Alignement of the skill between the three main stats
	private uint effect;			// ID of the entry in the effects table unlocking this skill applies
	// TODO: Position in the tree
	private ArrayList neighbours;	// List of skills this skill is connected to (type: uint)

	public Skill(string name,
		string description = null,
		SkillColor color = SkillColor.W,
		uint effect = 0,
		uint[] neighbours = null)
	{
		this.name = name;
		this.description = description;
		this.color = color;
		this.effect = effect;

		if (neighbours == null)
		{
			this.neighbours = new ArrayList();
		}
		else
		{
			this.neighbours = new ArrayList(neighbours);
		}
	}

	public string GetName()
	{
		return this.name;
	}

	public string GetDesc()
	{
		return this.description;
	}

	public SkillColor GetColor()
	{
		return this.color;
	}

	public Effect GetEffect()
	{
		return DataTables.GetEffect(this.effect);
	}

	public ArrayList GetNeighbours()
	{
		ArrayList res = new ArrayList(this.neighbours.Count);
		foreach (uint neighbour in this.neighbours)
		{
			res.Add(DataTables.GetSkill(neighbour));
		}
		return res;
	}
}
