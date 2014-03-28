using UnityEngine;
using System.Collections;

public class SpellProjHit : SpellTarget
{
	public SpellProjHit()
		: base(new Metadata("Impact de projectile", "Inflige les dégâts du projectile de l'arme à distance équipée"), SpellCostType.NONE)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		if (target != null)
		{
			target.GetCombatScript().ApplySpell(inflictor, this);
			inflictor.GetCombatScript().Damage(target, inflictor.GetStatsScript().GetProjDamage());
		}
	}

	public override float GetCost(CharacterManager caster)
	{
		return 0;
	}
}
