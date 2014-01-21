using UnityEngine;
using System.Collections;

public class ArmorSet
{
	private string name;			// Descriptive name of the set

	private uint skill;				// Index of the entry in the skill table completing this set gives access to
	private Stats stats;			// Bonus stats to be granted upon completing the set

	private uint setSize;			// Number of items in this set
	private bool autoSize;			// If true, "setSize" will increase automatically as items are created using this set

	public ArmorSet(string name,
		uint skill = 0,
		Stats stats = null,
		uint setSize = 0)
	{
		this.name = name;

		this.skill = skill;

		if (stats != null)
			this.stats = stats;
		else
			this.stats = new Stats();

		this.setSize = setSize;
		this.autoSize = (setSize == 0);
	}

	public void IncreaseSetSize()
	{
		if (this.autoSize)
		{
			this.setSize++;
		}
	}
}
