using UnityEngine;
using System.Collections;

public class MonsterEventScript : CharacterEventScript
{
	private NPCAIScript _ai;
	private MonsterMiscDataScript _misc;
	private CharacterInventoryScript _inventory;

	public override void Initialize(CharacterManager manager)
	{
		//base.Initialize(manager);
		_manager = manager;
		_ai = (NPCAIScript)_manager.GetInputScript();
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
		_inventory = _manager.GetInventoryScript();
	}

	public override void OnPain(float damage)
	{

	}

	public override void OnDeath(CharacterManager killer)
	{
		MonsterSpawnerScript spawner = _misc.GetSpawner();

		if (spawner)
		{
			spawner.OnBoundMonsterDeath();
		}

		_inventory.DropAllItems();

		if (Network.isClient || Network.isServer)
		{
			Network.Destroy(_manager.gameObject);
		}
		else
		{
			Destroy(_manager.gameObject);
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
