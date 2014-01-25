using UnityEngine;
using System.Collections;

public class Buff
{
	private string name;			// Name of the buff
	private ArrayList effects;		// List of effects this buff inflicts (also used for description) (type: uint)
	private string iconPath;		// Icon to display to represent this buff (NOTE: If null, the buff will not be displayed)

	public Buff(string name,
		uint[] effects = null,
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
