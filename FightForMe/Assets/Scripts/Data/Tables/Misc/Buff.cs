using UnityEngine;
using System.Collections;

public class Buff : WikiEntry
{
	private ArrayList effects;		// List of effects this buff inflicts (also used for description) (type: uint)

	public Buff(string name,
		string description = null,
		string iconPath = null,
		Quality quality = Quality.COMMON, // Is this necessary?
		uint[] effects = null)
		: base(name, null, description, null, iconPath, quality)
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
		this.description = "";
		foreach (uint effectID in this.effects)
		{
			Effect effect = DataTables.GetEffect(effectID);
			string color = (effect.IsPositive() ? "<green>" : "<red>");
			this.description += color + effect.GetDescription() + "\n";
		}
	}

	public ArrayList GetEffects()
	{
		return effects;
	}
}
