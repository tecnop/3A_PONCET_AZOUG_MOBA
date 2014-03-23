using UnityEngine;
using System.Collections;

public class SpellFireExplosion: SpellArea
{
	public SpellFireExplosion()
		: base(new Metadata("Explosion de feu", "Inflige des dégâts aux ennemis dans la zone et les brûle"))
	{

	}

	public override void Execute(CharacterManager inflictor, Vector3 position, CharacterManager target = null)
	{
 		// Use ability 4 here
	}
}