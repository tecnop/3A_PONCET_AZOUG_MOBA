using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buff : WikiEntry
{
	private List<uint> effects;		// List of effects this buff inflicts (also used for description)

	public override WikiCategory category
	{
		get
		{
			return WikiCategory.BUFFS;
		}
	}

	public Buff(Metadata metadata,
		uint[] effects = null)
		: base(metadata)
	{
		if (effects == null)
		{ // This shouldn't really happen...
			this.effects = new List<uint>();
		}
		else
		{
			this.effects = new List<uint>(effects);
		}
	}

	public List<uint> GetEffects()
	{
		return effects;
	}

	public override void DrawDataWindow(float width, float height)
	{
		base.DrawDataWindow(width, height);

		GUILayout.BeginArea(SRect.Make(0.0f, 0.45f * height, width, 0.55f * height, "data_window_buff_effects"));
		foreach (uint effectID in this.effects)
		{
			Effect effect = DataTables.GetEffect(effectID);
			GUILayout.Label(WikiEntry.ParseText(effect.GetDescription(), null), (effect.IsPositive() ? FFMStyles.positive : FFMStyles.negative));
		}
		GUILayout.EndArea();
	}

	public override void DrawWikiPage(float width, float height)
	{
		base.DrawWikiPage(width, height);

		// TODO: List of spells and weapons that apply this

		GUILayout.BeginArea(SRect.Make(0.0f, 0.5f * height, width, 0.5f * height, "wiki_buff_effects"));
		foreach (uint effectID in this.effects)
		{
			Effect effect = DataTables.GetEffect(effectID);
			GUILayout.Label(WikiEntry.ParseText(effect.GetDescription(), null), (effect.IsPositive() ? FFMStyles.positive : FFMStyles.negative));
		}
		GUILayout.EndArea();
	}
}
