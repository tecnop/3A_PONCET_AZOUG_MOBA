using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

public class Skill : WikiEntry
{
	private SkillColor color;		// Alignement of the skill between the three main stats
	private uint effect;			// ID of the entry in the effects table unlocking this skill applies
	// TODO: Position in the tree
	private List<uint> neighbours;	// List of skills this skill is connected to (type: uint)

	public Skill(Metadata metadata,
		SkillColor color = SkillColor.W,
		uint effect = 0,
		uint[] neighbours = null)
		: base(metadata)
	{
		this.color = color;
		this.effect = effect;

		if (neighbours == null)
		{
			this.neighbours = new List<uint>();
		}
		else
		{
			this.neighbours = new List<uint>(neighbours);
		}
	}

	public SkillColor GetColor()
	{
		return this.color;
	}

	public Effect GetEffect()
	{
		return DataTables.GetEffect(this.effect);
	}

	public List<Skill> GetNeighbours()
	{
		List<Skill> res = new List<Skill>(this.neighbours.Count);
		foreach (uint neighbour in this.neighbours)
		{
			res.Add(DataTables.GetSkill(neighbour));
		}
		return res;
	}
}
