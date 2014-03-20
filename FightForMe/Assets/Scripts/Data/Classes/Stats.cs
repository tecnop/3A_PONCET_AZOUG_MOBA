using UnityEngine;
using System.Collections;

public class Stats
{
	private uint strength;			// Affects max HP and HP/s
	private uint agility;			// Affects weapon damage
	private uint intelligence;		// Affects max MP and MP/s

	public Stats(uint strength = 0,
		uint agility = 0,
		uint intelligence = 0)
	{
		this.strength = strength;
		this.agility = agility;
		this.intelligence = intelligence;
	}

	public static Stats operator +(Stats a, Stats b)
	{
		return new Stats(a.strength+b.strength, a.agility+b.agility, a.intelligence+b.intelligence);
	}

	public static Stats operator -(Stats a, Stats b)
	{ // Stats cannot go negative for obvious reasons.
		uint finalStr = (a.strength > b.strength) ? a.strength - b.strength : 0;
		uint finalAgi = (a.agility > b.agility) ? a.agility - b.agility : 0;
		uint finalInt = (a.intelligence > b.intelligence) ? a.intelligence - b.intelligence : 0;

		return new Stats(finalStr, finalAgi, finalInt);
	}

	public Stats SetMin(Stats min)
	{ // Convinience function for characters, we don't want debuffs giving them close to 0 max HP, damage or max MP
		uint finalStr = (this.strength > min.strength) ? this.strength : min.strength;
		uint finalAgi = (this.agility > min.agility) ? this.agility : min.agility;
		uint finalInt = (this.intelligence > min.intelligence) ? this.intelligence : min.intelligence;

		return new Stats(finalStr, finalAgi, finalInt);
	}

	public static Stats Base { // Starting stats for every character
		get {
			return new Stats(10,10,10);
		}
	}

	public uint GetStrength()
	{
		return this.strength;
	}

	public uint GetAgility()
	{
		return this.agility;
	}

	public uint GetIntelligence()
	{
		return this.intelligence;
	}

	public override string ToString()
	{
		return "{ STR: "+strength+"; AGI: "+agility+"; INT: "+intelligence+" }";
	}
}