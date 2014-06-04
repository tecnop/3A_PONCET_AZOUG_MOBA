using UnityEngine;
using System.Collections;

public class MonsterEventScript : CharacterEventScript
{
	private NPCAIScript _ai;
	private MonsterMiscDataScript _misc;
	private CharacterInventoryScript _inventory;

	[SerializeField]
	private NetworkView _networkView;

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
		if (damage > 0)
		{
			_manager.GetCharacterAnimator().SetBool("onPain", true);
		}

		_ai.AcknowledgeTarget(inflictor);
	}

	public override void OnReceiveBuff(CharacterManager inflictor, uint buffID)
	{
		if (_ai != null)
		{
			_ai.AcknowledgeTarget(inflictor);
		}
	}

	public override void OnKnockback(CharacterManager inflictor)
	{
		_ai.AcknowledgeTarget(inflictor);
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

		if (GameData.gameMode == GameMode.RaceForGlory)
		{
			if (_misc.GetMonsterID() == 8)
			{ // Special dude died, give the trophy to the killer
				_manager.GetCombatScript().InflictBuff(killer, 4, 0);
			}
		}

		// No death animation for monsters yet
		if (GameData.isOnline)
		{
			_networkView.RPC("KillMe", RPCMode.Server);
		}
		else
		{
			Destroy(_manager.gameObject);
		}
	}

	[RPC]
	private void KillMe()
	{
		_networkView.RPC("DoKill", RPCMode.AllBuffered);
	}

	[RPC]
	private void DoKill()
	{
		//Network.Destroy(this.gameObject);
		Destroy(this.gameObject);
	}

	public override void OnSpotEntity(GameObject entity)
	{
		CharacterManager enemy = _ai.GetEnemy();

		if (enemy && enemy.gameObject == entity)
		{ // Regained sight of our target
			_ai.GainSightOfTarget();
		}
		else if (_ai.IsSearchingEnemy())
		{
			_ai.AcknowledgeTarget(entity);
		}
	}

	public override void OnLoseSightOfEntity(GameObject entity)
	{
		CharacterManager enemy = _ai.GetEnemy();

		if (enemy && enemy.gameObject == entity)
		{ // oh noooooo
			_ai.LoseSightOfTarget();
		}
	}

	public override void OnCollision(Collider collider)
	{

	}
}
