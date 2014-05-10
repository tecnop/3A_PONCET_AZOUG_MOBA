using UnityEngine;
using System.Collections;

public abstract class WikiEntry
{ // Each class that extends this class may have an associated entry in the game's wiki
	private Metadata metadata;

	protected WikiEntry(Metadata metadata)
	{
		this.metadata = metadata;
	}

	protected void EditDesc(string newDesc)
	{ // Only our children may edit their description
		metadata.SetDesc(newDesc);
	}

	private string ReadTag(string tag, CharacterManager manager)
	{
		Debug.Log(tag);
		return tag;
	}

	public string ParseDescription(CharacterManager manager)
	{ // TODO: Parses the item's short description and replace character-related tags with the required values
		string desc = metadata.GetDesc();
		string output = "";

		if (desc == null)
		{
			return null;
		}

		int pos = 0; // Last validated position
		for (int i = 0; i < desc.Length; i++)
		{
			if (desc[i] == '<')
			{
				output += desc.Substring(pos, i - pos + 1);
				pos = i;

				int j = i;
				while (j < desc.Length)
				{
					if (desc[j] == '>')
					{
						output += ReadTag(desc.Substring(pos, j - i + 1), manager);
						pos = j + 1;
						break;
					}
					j++;
				}
				if (desc[j] != '>')
				{ // Error: this tag is never closed
					return "PARSING ERROR";
				}
			}
		}

		output += desc.Substring(pos, desc.Length - pos);

		return output;
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

	public void DrawDataWindow(float width, float height)
	{ // Small data window rendered using GUI functions. Width and height will generally be around 400 (to be determined later)
		GUI.Label(new Rect(0.0f, 0.0f, width, height), "Entry " + GetName() + " does not have a data window display function", FFMStyles.centeredText_wrapped);
	}

	public void DrawWikiPage(float width, float height)
	{ // Large window rendered using GUI functions. Covers roughly 64% of the screen (80% in each dimension)
		GUI.Label(new Rect(0.0f, 0.0f, width, height), "Entry " + GetName() + " does not have a wiki page display function", FFMStyles.centeredText_wrapped);
	}
}