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
		_manager.GetCharacterAnimator().SetBool("onPain", true);

		if (!_ai.HasAnEnemy() &&
			inflictor.tag == "Player" &&
			inflictor.gameObject.layer != _manager.gameObject.layer)
		{
			Debug.Log(_manager.name + " acquired an enemy: " + inflictor.name);
			_ai.SetTarget(inflictor.gameObject);
		}
	}

	public override void OnReceiveBuff(CharacterManager inflictor, uint buffID)
	{
		if (!_ai.HasAnEnemy() &&
			inflictor.tag == "Player" &&
			inflictor.gameObject.layer != _manager.gameObject.layer)
		{
			Debug.Log(_manager.name + " acquired an enemy: " + inflictor.name);
			_ai.SetTarget(inflictor.gameObject);
		}
	}

	public override void OnDeath(CharacterManager killer)
	{
		_manager.GetCharacterAnimator().SetBool("isDead", true);

		SpawnerScript spawner = _misc.GetSpawner();

		if (spawner)
		{
			spawner.OnSpawnedEntityDeath();
		}

		_inventory.DropAllItems();

		if (_misc.GetMonsterID() == 8)
		{ // Special dude died, give the trophy to the killer
			_manager.GetCombatScript().InflictBuff(killer, 4, 0);
		}

		// No death animation for monsters yet
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
			entity.gameObject.layer != _manager.gameObject.layer)
		{
			Debug.Log(_manager.name + " acquired an enemy: " + entity.name);
			_ai.SetTarget(entity);
		}
	}

	public override void OnCollision(Collider collider)
	{

	}
}
