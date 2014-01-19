using UnityEngine;
using System.Collections;

public enum Effects
{

}

public class Buff
{
	private string name;
	private ArrayList effects;

	public Buff(string name,
		Effects[] effects)
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
