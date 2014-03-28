using UnityEngine;
using System.Collections;

public class SpellBurn : SpellTarget
{
	public SpellBurn()
		: base(new Metadata("Brûlure", "Inflige des dégâts sur la durée à la cible"), SpellCostType.NONE)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		inflictor.GetCombatScript().InflictBuff(target, 3, 5.0f);
	}

	public override float GetCost(CharacterManager caster)
	{
		return 0;
	}
}