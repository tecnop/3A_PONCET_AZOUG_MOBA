using UnityEngine;
using System.Collections;

public class SpellFireball : SpellProj
{
	public SpellFireball()
		: base(new Metadata("Boule de feu", "Lance un projectile enflammé qui explose au contact avec un obstacle, brûlant tout ce qui se trouve aux alentours"), SpellCostType.MANA)
	{

	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
		/*Vector3 pos = inflictor.GetCharacterTransform().position;
		Quaternion dir = Quaternion.LookRotation(position - pos);
		CreateProjectile(inflictor, pos, dir, 2);*/
		inflictor.GetCombatScript().CreateProjectile(2);
	}

	public override float GetCost(CharacterManager caster)
	{
		return 50;
	}
}