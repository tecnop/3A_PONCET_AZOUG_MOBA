using UnityEngine;
using System.Collections;

public class SpellExplosion : SpellArea
{
	public SpellExplosion()
		: base(new Metadata("Explosion", "Expulse tous les ennemis vers l'exterieur de la zone"))
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		inflictor.GetCombatScript().CreateAoE(position, Quaternion.identity, 8.0f, 9);
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
