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

	public override void OnPain(CharacterManager inflictor, float damage)
	{
		_manager.GetCharacterAnimator ().SetBool ("onPain", true);

		if (!_ai.HasAnEnemy() &&
			inflictor.tag == "Player" &&
			LayerMask.LayerToName(inflictor.gameObject.layer) != "NeutralEntity")
		{
			Debug.Log("Monster acquired an enemy: " + inflictor.name);
			_ai.SetTarget(inflictor.gameObject);
		}
	}

	public override void OnDeath(CharacterManager killer)
	{
		_manager.GetCharacterAnimator().SetBool("isDead", true);

		MonsterSpawnerScript spawner = (MonsterSpawnerScript)_misc.GetSpawner();

		if (spawner)
		{
			spawner.OnSpawnedEntityDeath();
		}

		_inventory.DropAllItems();

		if (GameData.isOnline)
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
			LayerMask.LayerToName(entity.layer) != "NeutralEntity")
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
