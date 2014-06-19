using UnityEngine;
using System.Collections;

public abstract class SpellTarget : Spell
{
	protected SpellTarget(Metadata metadata, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f, string castingSound = null)
		: base(metadata, SpellType.TARGET, costType, spellCost, castingSound)
	{

	}

	protected abstract void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null);

	public override void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		if (target != null)
		{
			target.GetCombatScript().ApplySpell(inflictor, this);
			this._Execute(inflictor, position, target);
		}
	}
}