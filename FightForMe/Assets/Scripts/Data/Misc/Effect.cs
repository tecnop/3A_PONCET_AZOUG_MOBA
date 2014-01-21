using UnityEngine;
using System.Collections;

public class Effect
{
	private string description;		// Descriptive text of the effect
	private bool isPositive;		// If true, the effect is an improvement, otherwise it is negative

	public Effect(string description,
		bool isPositive)
	{
		this.description = description;
		this.isPositive = isPositive;
	}
}