using UnityEngine;
using System.Collections;

public class SpellRegeneration : SpellTarget
{
	public SpellRegeneration()
		: base(new Metadata("Regénération", "La cible récupère sa vie progressivement mais subit des dégâts plus élevés"), SpellCostType.MANA)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{ // TODO: Toggle
		//inflictor.GetCombatScript().InflictBuff(target, 3, 5.0f);
	}

	public override float GetCost(CharacterManager caster)
	{
		return 0;
	}

	public override bool CastingCondition(CharacterManager caster)
	{
		return true;
	}
}