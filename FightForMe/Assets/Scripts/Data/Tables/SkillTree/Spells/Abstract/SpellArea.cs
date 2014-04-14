using UnityEngine;
using System.Collections;

public abstract class SpellArea : Spell
{
	protected SpellArea(Metadata metadata, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f)
		: base(metadata, SpellType.AREA, costType, spellCost)
	{

	}

	protected abstract void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null);

	public override void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		//if ((inflictor.GetCharacterTransform().position - position).magnitude < GetRange(inflictor)) // Doesn't seem like the right place to do this! Put that in UseSpell() in CharacterCombatScript instead
		if (true)
		{
			this._Execute(inflictor, position, target);
		}
	}

	//public abstract float GetRange(CharacterManager caster);
}