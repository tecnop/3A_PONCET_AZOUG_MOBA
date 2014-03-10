using UnityEngine;
using System.Collections;

/*
 * CharacterCombatScript.cs
 * 
 * Allows characters to "interact" with each other.
 * 
 */

public class CharacterCombatScript : MonoBehaviour
{
	[SerializeField]
	private GameObject damageSpherePrefab;

	[SerializeField]
	private GameObject projectilePrefab;

	private CharacterManager _manager;

	private Transform _transform;

	private ArrayList buffs;		// List of active buffs and debuffs (type: InflictedBuff)
	// TODO: Add a list of effects built from the list of buffs for quicker access? Would building it be more expensive than accessing each entry individually...?

	private ArrayList combatLog;	// List of previously applied damage instances, cleared upon respawn (type: DamageInstance)

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = this.transform;

		if (buffs == null)
		{
			buffs = new ArrayList();
		}
	}

	public void Damage(CharacterManager target, float damage, Vector3 damageDir = new Vector3(), uint damageFlags = 0)
	{
		target.GetStatsScript().LoseHealth(_manager, damage);
		// TODO: Knockback, check other flags
	}

	public void Damage(GameObject target, float damage, Vector3 damageDir = new Vector3(), uint damageFlags = 0)
	{
		CharacterManager hisManager = target.GetComponent<CharacterManager>();
		if (hisManager)
		{
			this.Damage(hisManager, damage, damageDir, damageFlags);
		}
	}

	public void AreaOfEffect(Vector3 position, Quaternion angle, float radius, float damage = 0.0f, uint buffID = 0, uint damageFlags = 0)
	{
		GameObject sphere;
		/*if (GameData.isOnline)
		{ // No longer used as this object is entirely run by prediction
			sphere = (GameObject)Network.Instantiate(damageSpherePrefab, position, angle, 0);
		}
		else*/
		{
			sphere = (GameObject)Instantiate(damageSpherePrefab, position, angle);
		}
		DetectionSphereScript sphereScript = sphere.GetComponent<DetectionSphereScript>();
		sphereScript.storeData(_manager, position, radius, _manager.GetLayer(), damage, buffID, damageFlags);
	}

	public void ShootProjectile(uint projectileID)
	{
		GameObject proj;
		/*if (GameData.isOnline)
		{ // No longer used as this object is entirely run by prediction
			proj = (GameObject)Network.Instantiate(projectilePrefab, _transform.position, _transform.rotation, 0);
		}
		else*/
		{
			proj = (GameObject)Instantiate(projectilePrefab, _transform.position, _transform.rotation);
		}
		ProjectileScript projScript = proj.GetComponent<ProjectileScript>();
		projScript.SetUpFromProjectile(projectileID);
		projScript.StoreData(_manager, _manager.GetLayer(), _manager.GetStatsScript().GetDamage());
	}

	public void DoMeleeAttack()
	{ // TODO: Inflict buffs
		AreaOfEffect(_transform.position, _transform.rotation, 2.0f, _manager.GetStatsScript().GetDamage());
	}

	public void DoAttack()
	{ // Main attack function
		Weapon myWeapon = _manager.GetInventoryScript().GetWeapon();
		
		if (myWeapon == null)
		{ // Using our fists
			DoMeleeAttack();
			return;
		}

		WeaponType type = myWeapon.GetWeaponType();
		if (type == null || !type.IsRanged())
		{
			DoMeleeAttack();
		}

		uint proj = myWeapon.GetProjectileID();
		if (proj != 0)
		{
			ShootProjectile(proj);
		}
	}

	public void ReceiveBuff(CharacterManager inflictor, uint buffID, float duration)
	{
		if (DataTables.GetBuff(buffID) == null)
		{ // DEBUG
			return;
		}

		this.buffs.Add(new InflictedBuff(buffID, duration, inflictor));
		_manager.GetStatsScript().UpdateStats();
	}

	public void InflictBuff(CharacterManager target, uint buffID, float duration)
	{
		target.GetCombatScript().ReceiveBuff(_manager, buffID, duration);
	}

	[RPC]
	public void CreateDamageInstance(CharacterManager inflictor, float damage, uint buffID, float buffDuration)
	{
		DamageInstance instance = new DamageInstance(inflictor, damage, buffID, buffDuration);

	}

	public void ApplyDamageInstance(DamageInstance instance)
	{ // This is stupid but at least I don't need to make getters :3
		DamageInstance appliedDamage = instance.ApplyToTarget(_manager);
		this.combatLog.Add(appliedDamage);
		appliedDamage.ApplyEffects();
	}
	// Create a remote function that takes every field of the DamageInstance class and applies a new local damage instance

	public void ResetCombatLog()
	{
		this.combatLog.Clear();
	}

	public void UpdateBuffs()
	{
		bool updated = false;
		for (int i = 0; i < buffs.Count; i++)
		{
			InflictedBuff buff = (InflictedBuff)buffs[i];
			if (GameData.gamePaused)
			{ // Don't lose the time spent with the game paused
				buff.AddToDuration(Time.deltaTime);
			}

			if (buff.GetTimeLeft() <= 0.0f)
			{
				buffs.RemoveAt(i);
				i--;
				updated = true;
			}
		}

		if (updated)
		{
			_manager.GetStatsScript().UpdateStats();
		}
	}

	public ArrayList GetBuffs()
	{
		return this.buffs;
	}
}
