using UnityEngine;
using System.Collections;

public class SpellAlacrity : SpellTarget
{
	public SpellAlacrity()
		: base(new Metadata("Alacrité", "Augmente la vitesse d'attaque et de déplacement temporairement"))
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		inflictor.GetCombatScript().InflictBuff(target, 5, 10.0f);
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