using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buff : WikiEntry
{
	private List<uint> effects;		// List of effects this buff inflicts (also used for description)

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

		// DEBUG
		metadata.SetDesc(BuildDescription());
	}

	protected override string BuildDescription()
	{
		if (this.effects == null) return null;

		List<string> descriptions = new List<string>();
		foreach (uint effectID in this.effects)
		{
			Effect effect = DataTables.GetEffect(effectID);
			//string color = (effect.IsPositive() ? "<green>" : "<red>");
			descriptions.Add(/*color + */effect.GetDescription());
		}

		return string.Join("\n", descriptions.ToArray());
	}

	public List<uint> GetEffects()
	{
		return effects;
	}

	public override void DrawWikiPage(float width, float height)
	{ // Testing!
		GUI.Label(new Rect(0.0f, 0.0f, width, height), this.GetDesc(), FFMStyles.centeredText_wrapped);
	}
}
