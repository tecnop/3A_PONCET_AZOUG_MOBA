using UnityEngine;
using System.Collections;

public class SpellFireExplosion: SpellArea
{
	//private Spell burn;

	public SpellFireExplosion()
		: base(new Metadata("Explosion de feu", "Inflige des dégâts aux ennemis dans la zone et les brûle"))
	{
		//this.burn = DataTables.GetSpell(6);
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		inflictor.GetCombatScript().CreateAoE(position, Quaternion.identity, 4.0f, 6); // burn
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