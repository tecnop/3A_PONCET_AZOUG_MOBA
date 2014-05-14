using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class WikiEntry
{ // Each class that extends this class may have an associated entry in the game's wiki
	private Metadata metadata;

	protected WikiEntry(Metadata metadata)
	{
		this.metadata = metadata;

		if (metadata.GetDesc() == null)
		{ // Build an automatic one
			metadata.SetDesc(BuildDescription());
		}
	}

	/*protected void EditDesc(string newDesc)
	{ // Only our children may edit their description (no longer necessary)
		metadata.SetDesc(newDesc);
	}*/

	private string ReadTag(string tag, CharacterManager manager)
	{
		string strippedTag = tag.Substring(1, tag.Length - 2);
		string[] parts = strippedTag.Split(' ');

		string result = null;

		if (parts.Length == 2)
		{
			string name = parts[0];

			float value;

			try
			{
				value = float.Parse(parts[1]);
			}
			catch (System.Exception e)
			{
				return "<PARSING ERROR: " + e.Message + ">";
			}

			if (manager)
			{
				float stat;

				if (name.Equals("health"))
				{
					stat = manager.GetStatsScript().GetHealth();
				}
				else if (name.Equals("maxhealth"))
				{
					stat = manager.GetStatsScript().GetMaxHealth();
				}
				else if (name.Equals("mana"))
				{
					stat = manager.GetStatsScript().GetMana();
				}
				else if (name.Equals("maxmana"))
				{
					stat = manager.GetStatsScript().GetMaxMana();
				}
				else if (name.Equals("movespeed"))
				{
					stat = manager.GetStatsScript().GetMovementSpeed();
				}
				else if (name.Equals("dmg"))
				{
					stat = manager.GetStatsScript().GetDamage();
				}
				else if (name.Equals("atkspd"))
				{
					stat = manager.GetStatsScript().GetAttackRate();
				}
				else if (name.Equals("projdmg"))
				{
					stat = manager.GetStatsScript().GetProjDamage();
				}
				else if (name.Equals("str"))
				{
					stat = manager.GetStatsScript().GetStats().GetStrength();
				}
				else if (name.Equals("agi"))
				{
					stat = manager.GetStatsScript().GetStats().GetAgility();
				}
				else if (name.Equals("int"))
				{
					stat = manager.GetStatsScript().GetStats().GetIntelligence();
				}
				else
				{
					return "<PARSING ERROR: Unrecognized tag>";
				}

				result = (value * stat).ToString();
			}

			string desc = (value * 100).ToString();

			if (name.Equals("health"))
			{
				desc += "% de ses points de vie actuels";
			}
			else if (name.Equals("maxhealth"))
			{
				desc += "% de ses points de vie maximum";
			}
			else if (name.Equals("mana"))
			{
				desc += "% de ses points de mana actuels";
			}
			else if (name.Equals("maxmana"))
			{
				desc += "% de ses points de mana maximum";
			}
			else if (name.Equals("movespeed"))
			{
				desc += "% de sa vitesse de déplacement actuelle";
			}
			else if (name.Equals("dmg"))
			{
				desc += "% de ses dégâts";
			}
			else if (name.Equals("atkspd"))
			{
				desc += "% de sa vitesse d'attaque";
			}
			else if (name.Equals("projdmg"))
			{
				desc += "% des dégâts de ses projectiles";
			}
			else if (name.Equals("str"))
			{
				desc += "% de son endurance";
			}
			else if (name.Equals("agi"))
			{
				desc += "% de sa puissance";
			}
			else if (name.Equals("int"))
			{
				desc += "% de son intelligence";
			}
			else
			{
				return "<PARSING ERROR: Unrecognized tag>";
			}

			if (result != null)
			{
				result += "(" + desc + ")";
			}
			else
			{
				result = desc;
			}
		}
		else
		{
			return "<PARSING ERROR: Too many arguments>";
		}

		return result;
	}

	public string ParseDescription(CharacterManager manager)
	{
		string desc = metadata.GetDesc();
		List<string> output = new List<string>();

		if (desc == null)
		{
			return null;
		}

		int pos = 0; // Last validated position
		for (int i = 0; i < desc.Length; i++)
		{
			if (desc[i] == '<')
			{
				output.Add(desc.Substring(pos, i - pos));
				pos = i;

				int j = i;
				while (j < desc.Length)
				{
					if (desc[j] == '>')
					{
						output.Add(ReadTag(desc.Substring(pos, j - i + 1), manager));
						pos = j + 1;
						break;
					}
					j++;
				}
				if (desc[j] != '>')
				{ // Error: this tag is never closed
					return "PARSING ERROR: A tag is never closed";
				}
			}
		}

		output.Add(desc.Substring(pos, desc.Length - pos));

		return string.Join("", output.ToArray());
	}

	public string GetName()
	{
		return metadata.GetName();
	}

	public string GetDesc()
	{
		return this.ParseDescription(null);
	}

	public string GetLongDesc()
	{
		return this.GetDesc() + "\n\n" + metadata.GetDesc2();
	}

	public GameModel GetModel()
	{
		return metadata.GetModel();
	}

	/*public GameObject GetModel()
	{
		GameModel model = metadata.GetModel();
		if (model != null)
		{
			return model.GetModel();
		}
		return null;
	}

	public string GetModelPath()
	{ // Shouldn't be needed anymore, oh well
		GameModel model = metadata.GetModel();
		if (model != null)
		{
			return model.GetModelPath();
		}
		return null;
	}*/

	public string GetIcon()
	{
		return metadata.GetIcon();
	}

	public Quality GetQuality()
	{
		return metadata.GetQuality();
	}

	protected virtual string BuildDescription()
	{
		return "<Description manquante>";
	}

	public virtual void DrawDataWindow(float width, float height)
	{ // Small data window rendered using GUI functions. Width and height will generally be around 400 (to be determined later)
		GUI.Label(new Rect(0.0f, 0.0f, width, height), "Entry " + GetName() + " does not have a data window display function", FFMStyles.centeredText_wrapped);
	}

	public virtual void DrawWikiPage(float width, float height)
	{ // Large window rendered using GUI functions. Covers roughly 64% of the screen (80% in each dimension)
		GUI.Label(new Rect(0.0f, 0.0f, width, height), "Entry " + GetName() + " does not have a wiki page display function", FFMStyles.centeredText_wrapped);
	}
}