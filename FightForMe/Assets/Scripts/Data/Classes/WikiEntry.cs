using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class WikiEntry
{ // Each class that extends this class may have an associated entry in the game's wiki
	private Metadata metadata;

	public virtual WikiCategory category
	{
		get
		{
			return WikiCategory.NONE;
		}
	}

	protected WikiEntry(Metadata metadata)
	{
		this.metadata = metadata;

		/*if (metadata.GetDesc() == null)
		{ // Build an automatic one
			metadata.SetDesc(BuildDescription());
		}*/
	}

	/*protected void EditDesc(string newDesc)
	{ // Only our children may edit their description (no longer necessary)
		metadata.SetDesc(newDesc);
	}*/

	private static string ReadTag(string tag, CharacterManager manager)
	{
		string strippedTag = tag.Substring(1, tag.Length - 2);
		string[] parts = strippedTag.Split(' ');

		string result = null;

		if (parts.Length == 2 || parts.Length == 3)
		{
			string name = parts[0];
			bool shortVersion = false;

			if (parts.Length == 3)
			{
				if (parts[2].Equals("/"))
				{ // Not very informative but meh
					shortVersion = true;
				}
			}

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

			if (shortVersion && result == null)
			{ // FIXME: If it's not a recognized tag? Probably won't happen but...
				desc += "%";
			}
			else if (name.Equals("health"))
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

	public static string ParseText(string text, CharacterManager activePlayer)
	{
		List<string> output = new List<string>();

		if (text == null)
		{
			return null;
		}

		int pos = 0; // Last validated position
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == '<')
			{
				output.Add(text.Substring(pos, i - pos));
				pos = i;

				int j = i;
				while (j < text.Length)
				{
					if (text[j] == '>')
					{
						output.Add(ReadTag(text.Substring(pos, j - i + 1), activePlayer));
						pos = j + 1;
						break;
					}
					j++;
				}
				if (j >= text.Length)
				{ // This tag is never closed
					return "PARSING ERROR: A tag is never closed";
				}
			}
		}

		output.Add(text.Substring(pos, text.Length - pos));

		return string.Join("", output.ToArray());
	}

	public string GetName()
	{
		return metadata.GetName();
	}

	public string GetDesc()
	{
		return WikiEntry.ParseText(metadata.GetDesc(), null);
	}

	public string GetLore()
	{
		return metadata.GetLore();
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

	public virtual void DrawDataWindow(float width, float height)
	{ // Small data window rendered using GUI functions. Width and height will generally be around 400 (to be determined later)
		if (GUI.Button(new Rect(0.0f, 0.0f, 20.0f, 20.0f), "?"))
		{
			HUDRenderer.SetState(HUDState.Wiki);
			WikiManager.SetEntry(this);
		}

		if (this.GetName() != null)
		{
			GUI.Label(new Rect(width / 3.0f, 0.0f, width / 3.0f, height / 5.0f), this.GetName(), FFMStyles.StyleForQuality(this.GetQuality(), false));
		}

		// TODO: Icon

		//GUI.Box(new Rect(0.0f, height / 5.0f, width, height / 4.0f), GUIContent.none);
		if (this.GetDesc() != null)
		{ // Parse the description? It's costly and not really necessary
			GUI.Label(new Rect(10.0f, height / 5.0f, width, height / 4.0f), this.GetDesc());
		}
	}

	public virtual void DrawWikiPage(float width, float height)
	{ // Large window rendered using GUI functions. Covers roughly 64% of the screen (80% in each dimension)
		if (this.GetName() != null)
		{
			GUI.Label(new Rect(0.0f, 0.0f, width, 0.15f * height), this.GetName(), FFMStyles.StyleForQuality(this.GetQuality(), true));
		}

		// TODO: Icon

		if (this.GetDesc() != null)
		{ // Parse the description? It's costly and not really necessary
			GUI.Label(new Rect(0.0f, 0.2f * height, width, 0.3f * height), this.GetDesc(), FFMStyles.textBlock);
		}

		if (this.GetLore() != null)
		{
			GUI.Label(new Rect(0.0f, 0.55f * height, width, 0.4f * height), this.GetLore(), FFMStyles.loreBlock);
		}
	}
}