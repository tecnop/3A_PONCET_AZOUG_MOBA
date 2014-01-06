using UnityEngine;
using System.Collections;

public class MonsterEventScript : CharacterEventScript
{
	private NPCAIScript _ai;
	private MonsterMiscDataScript _misc;

	void Start()
	{
		_ai = (NPCAIScript)_manager.GetInputScript();
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
	}

	public override void OnPain(float damage)
	{

	}

	public override void OnDeath(CharacterManager killer)
	{ // TODO: Drop our items
		MonsterSpawnerScript spawner = _misc.GetSpawner();

		if (spawner)
		{
			spawner.OnBoundMonsterDeath();
		}
	}

	public override void OnSpotEntity(GameObject entity)
	{
		if (_ai.IsSearchingEnemy() &&
			entity.tag == "Player" &&
			LayerMask.LayerToName(entity.layer) == "Team1Entity") // Temporary
		{
			Debug.Log("Monster acquired an enemy: " + entity.name);
			_ai.SetTarget(entity);
		}
	}

	public override void OnCollision(Collider collider)
	{
		if (collider.tag == "WayPoint")
		{
			WayPointScript wp = collider.GetComponent<WayPointScript>();
			_ai.OnWayPointCollision(wp.GetID());
		}
	}
}
