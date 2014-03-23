using UnityEngine;
using System.Collections;

public class SpellBurn : SpellTarget
{
	public SpellBurn()
		: base(new Metadata("Brûlure", "Inflige des dégâts sur la durée à la cible"))
	{

	}

	public override void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		if (target != null)
		{
			inflictor.GetCombatScript().InflictBuff(target, 3, 5.0f);
		}
	}
}