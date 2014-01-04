using UnityEngine;
using System.Collections;

public class ArmorSet
{
	private string name;			// Descriptive name of the set

	private uint skill;				// Index of the entry in the skill table completing this set gives access to
	private Stats stats;			// Bonus stats to be granted upon completing the set

	public ArmorSet(string name)
	{
		this.name = name;
		this.skill = 0;
		this.stats = new Stats(5, 5, 5);
	}
}
