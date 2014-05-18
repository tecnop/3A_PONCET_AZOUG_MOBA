using UnityEngine;
using System.Collections;

public enum Quality // It describes quality using rarity adjectives because consistency is for losers
{
	JUNK,		// Gray
	COMMON,		// White
	RARE,		// Purple
	EPIC,		// Orange
	UNIQUE		// Red and black
}

public class Metadata
{
	private string name;			// Entry name
	private string description;		// Quite short, generally an overview of stats and such
	private string lore;			// May be longer and contain less gameplay-related information such as lore, jokes, tips...
	private GameModel model;		// Game model associated with this entry
	private string iconPath;		// Path to the picture associated with this entry
	private Quality quality;		// Quality level of the associated object, changes the color of the title

	public Metadata(string name,
		string description = null,
		string lore = null,
		string modelPath = null,
		float scale = 1.0f,
		string iconPath = null,
		Quality quality = Quality.COMMON)
	{
		this.name = name;
		this.description = description;
		this.lore = lore;
		//if (modelPath != null)
		{
			this.model = new GameModel(modelPath, scale);
		}
		/*else
		{
			this.model = null;
		}*/
		this.iconPath = iconPath;
		this.quality = quality;
	}

	public string GetName()
	{
		return this.name;
	}

	public string GetDesc()
	{
		return this.description;
	}

	public string GetLore()
	{
		return this.lore;
	}

	public void SetDesc(string newDesc)
	{
		this.description = newDesc;
	}

	public GameModel GetModel()
	{
		return this.model;
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