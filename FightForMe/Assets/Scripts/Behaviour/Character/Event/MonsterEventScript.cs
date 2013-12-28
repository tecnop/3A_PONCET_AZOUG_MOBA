using UnityEngine;
using System.Collections;

public class MonsterEventScript : CharacterEventScript {

	public override void OnPain(float damage)
	{

	}

	public override void OnDeath(GameObject killer)
	{ // Drop our items

	}

	public override void OnSpotEnemy(GameObject enemy)
	{

	}

	public override void OnCollision(Collider collider)
	{ // TODO: Pathfinding stuff
	}
}
