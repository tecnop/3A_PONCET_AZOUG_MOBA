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

	public string ParseDescription(CharacterManager manager)
	{ // TODO: Parses the item's short description and replace character-related tags with the required values
		return metadata.GetDesc();
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

	public GameObject GetModel()
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
	}

	public string GetIcon()
	{
		return metadata.GetIcon();
	}

	public Quality GetQuality()
	{
		return metadata.GetQuality();
	}
}