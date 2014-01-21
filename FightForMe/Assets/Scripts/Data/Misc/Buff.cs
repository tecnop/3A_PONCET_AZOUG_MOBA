using UnityEngine;
using System.Collections;

public class Buff
{
	private string name;			// Name of the buff
	private ArrayList effects;		// List of effects this buff inflicts (also used for description)
	private Stats stats;			// Stat modifiers of this buff
	private string iconPath;		// Icon to display to represent this buff

	public Buff(string name,
		Effect[] effects = null,
		Stats stats = null,
		string iconPath = null)
	{
		this.name = name;

		if (effects == null)
		{
			this.effects = new ArrayList();
		}
		else
		{
			this.effects = new ArrayList(effects);
		}

		if (stats == null)
		{
			this.stats = new Stats();
		}
		else
		{
			this.stats = stats;
		}

		this.iconPath = iconPath;
	}

	public string GetName()
	{
		return name;
	}

	public ArrayList GetEffects()
	{
		return effects;
	}
}
