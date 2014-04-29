using UnityEngine;
using System.Collections;

public class SpellToggleBuff : SpellTarget
{
	private uint buffID;

	public SpellToggleBuff(Metadata metadata, uint buffID, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f)
		: base(metadata, costType, spellCost)
	{
		this.buffID = buffID;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{ // TODO: Somehow consume the specified resource over the duration of the buff
		if (target.GetCombatScript().HasBuff(buffID))
		{
			target.GetCombatScript().RemoveBuff(buffID);
		}
		else
		{
			inflictor.GetCombatScript().InflictBuff(target, buffID);
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}