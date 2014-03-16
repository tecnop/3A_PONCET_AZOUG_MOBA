using UnityEngine;
using System.Collections;

public enum Quality // It describes quality using rarity adjectives because consistency is for losers
{
	JUNK,		// Gray
	COMMON,		// White
	UNCOMMON,	// Green
	RARE,		// Blue
	EPIC,		// Purple
	UNIQUE		// Orange
}

public abstract class WikiEntry
{ // Each class that extends this class may have an associated entry in the game's wiki
	protected string name;			// Entry name
	protected string description;	// Quite short, generally stats and such
	protected string description2;	// May be longer and contain less important information such as lore, jokes, tips...
	protected GameModel model;		// Game model associated with this entry
	protected string iconPath;		// Path to the picture associated with this entry
	protected Quality quality;		// Quality level of the associated object, changes the color of the title

	protected WikiEntry(string name,
		string description,
		string description2 = null,
		GameModel model = null,
		string iconPath = null,
		Quality quality = Quality.COMMON)
	{
		this.name = name;
		this.description = description;
		this.description2 = description2;
		this.model = model;
		this.iconPath = iconPath;
		this.quality = quality;
	}

	public string ParseDescription(CharacterManager manager)
	{ // TODO: Parses the item's short description and replace character-related tags with the required values
		return this.description;
	}

	public string GetName()
	{
		return this.name;
	}

	public string GetDesc()
	{
		return this.ParseDescription(null);
	}

	public string GetLongDesc()
	{
		return this.GetDesc() + "\n\n" + this.description2;
	}

	public GameObject GetModel()
	{
		return this.model.GetModel();
	}

	public string GetModelPath()
	{ // Shouldn't be needed anymore, oh well
		return this.model.GetModelPath();
	}

	public string GetIcon()
	{
		return this.iconPath;
	}

	public Quality GetQuality()
	{
		return this.quality;
	}
}