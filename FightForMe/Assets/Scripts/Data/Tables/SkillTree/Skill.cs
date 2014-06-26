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
	W	// Neutral (starting skill only please! it doesn't make sense for anything else!)
}

public class Skill : WikiEntry
{
	private Vector2 treePos;		// Our position in the skill tree
	private SkillColor color;		// Alignement of the skill between the three main stats
	private uint effect;			// ID of the entry in the effects table unlocking this skill applies
	// TODO: Position in the tree
	private List<uint> neighbours;	// List of skills this skill is connected to

	public override WikiCategory category
	{
		get
		{
			return WikiCategory.SKILLS;
		}
	}

	public Skill(Metadata metadata,
		Vector2 treePos,
		SkillColor color = SkillColor.W,
		uint effect = 0,
		uint[] neighbours = null)
		: base(metadata)
	{
		this.treePos = treePos;
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

	public Vector2 GetTreePos()
	{
		return this.treePos;
	}

	public List<Skill> GetNeighbours()
	{ // TODO: Do this once and for all
		List<Skill> res = new List<Skill>(this.neighbours.Count);
		foreach (uint neighbour in this.neighbours)
		{
			res.Add(DataTables.GetSkill(neighbour));
		}
		return res;
	}

	public override void DrawDataWindow(float width, float height)
	{
		if (this.GetName() != null)
		{
			GUI.Label(SRect.Make(width / 3.0f, 0.0f, width / 3.0f, height / 3.0f, "data_window_skill_name"), this.GetName(), FFMStyles.StyleForQuality(this.GetQuality(), false));
		}

		if (this.color == SkillColor.R)
		{
			GUIStyle style = new GUIStyle(FFMStyles.centeredText_wrapped);
			style.normal.textColor = Color.red;
			GUI.Label(SRect.Make(2.0f * width / 3.0f, 0.0f, width / 3.0f, height / 3.0f, "data_window_skill_type"), "Endurance", style);
		}
		else if (this.color == SkillColor.G)
		{
			GUIStyle style = new GUIStyle(FFMStyles.centeredText_wrapped);
			style.normal.textColor = Color.green;
			GUI.Label(SRect.Make(2.0f * width / 3.0f, 0.0f, width / 3.0f, height / 3.0f, "data_window_skill_type"), "Puissance", style);
		}
		else if (this.color == SkillColor.B)
		{
			GUIStyle style = new GUIStyle(FFMStyles.centeredText_wrapped);
			style.normal.textColor = Color.blue;
			GUI.Label(SRect.Make(2.0f * width / 3.0f, 0.0f, width / 3.0f, height / 3.0f, "data_window_skill_type"), "Intelligence", style);
		}
		else
		{
			GUI.Label(SRect.Make(2.0f * width / 3.0f, 0.0f, width / 3.0f, height / 3.0f, "data_window_skill_type"), "Hybride", FFMStyles.centeredText_wrapped);
		}

		Effect effect = DataTables.GetEffect(this.effect);
		if (effect != null)
		{ // Colour it? Not sure if it's a good idea
			GUI.Label(SRect.Make(0.0f, height / 3.0f, width, height / 3.0f, "data_window_skill_effect"), WikiEntry.ParseText(effect.GetDescription(), GameData.activePlayer), FFMStyles.centeredText_wrapped);

			Spell unlockedSpell = DataTables.GetSpell(effect.GetUnlockedAbility());
			if (unlockedSpell != null && unlockedSpell.GetDesc() != null)
			{
				GUI.Label(SRect.Make(0.0f, 2.0f * height / 3.0f, width, height / 3.0f, "data_window_skill_ability"), unlockedSpell.GetName() + ": " + WikiEntry.ParseText(unlockedSpell.GetDesc(), GameData.activePlayer), FFMStyles.centeredText_wrapped);
			}
		}
	}

	public override void DrawWikiPage(float width, float height)
	{
		base.DrawWikiPage(width, height);
	}
}
