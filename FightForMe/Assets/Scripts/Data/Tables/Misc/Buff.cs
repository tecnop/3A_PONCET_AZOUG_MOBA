using UnityEngine;
using System.Collections;

public class Buff : WikiEntry
{
	private ArrayList effects;		// List of effects this buff inflicts (also used for description) (type: uint)

	public Buff(Metadata metadata,
		uint[] effects = null)
		: base(metadata)
	{
		if (effects == null)
		{ // This shouldn't really happen...
			this.effects = new ArrayList();
		}
		else
		{
			this.effects = new ArrayList(effects);
		}

		// Set our short description to list our effects (note: this is the short description in WikiEntry, the one passed as parameter goes to description2)
		string description = "";
		foreach (uint effectID in this.effects)
		{
			Effect effect = DataTables.GetEffect(effectID);
			string color = (effect.IsPositive() ? "<green>" : "<red>");
			description += color + effect.GetDescription() + "\n";
		}

		this.EditDesc(description);
	}

	public ArrayList GetEffects()
	{
		return effects;
	}
}
