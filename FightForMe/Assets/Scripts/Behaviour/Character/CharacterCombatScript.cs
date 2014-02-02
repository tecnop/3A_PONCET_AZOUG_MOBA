using UnityEngine;
using System.Collections;

/*
 * CharacterCombatScript.cs
 * 
 * Allows characters to "interact" with each other.
 * 
 */

public class CharacterCombatScript : MonoBehaviour
{ // NOTE FOR RELEASE: Create a DamageInstance class for a cleaner code and combat logging
	
	[SerializeField]
	private GameObject damageSpherePrefab;

	private CharacterManager _manager;

	private ArrayList buffs;		// List of active buffs and debuffs (type: InflictedBuff)
	// TODO: Add a list of effects built from the list of buffs for quicker access? Would building it be more expensive than accessing each entry individually...?

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;

		if (buffs == null)
		{
			buffs = new ArrayList();
		}
	}

	public void Damage(CharacterManager target, float damage, Vector3 damageDir = new Vector3(), uint damageFlags = 0)
	{
		target.GetStatsScript().loseHealth(_manager, damage);
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
		GameObject sphere = (GameObject)Network.Instantiate(damageSpherePrefab, position, angle, 0);
		DetectionSphereScript sphereScript = sphere.GetComponent<DetectionSphereScript>();
		sphereScript.storeData(_manager, position, radius, this.gameObject.layer, damage, buffID, damageFlags);
		//ArrayList array = sphereScript.GetResults();
	}

	public void ReceiveBuff(CharacterManager inflictor, uint buffID, float duration)
	{
		this.buffs.Add(new InflictedBuff(buffID, duration, inflictor));
		_manager.GetStatsScript().UpdateStats();
	}

	public void InflictBuff(CharacterManager target, uint buffID, float duration)
	{
		target.GetCombatScript().ReceiveBuff(_manager, buffID, duration);
	}

	public void UpdateBuffs()
	{
		bool updated = false;
		for (int i = 0; i < buffs.Count; i++)
		{
			if (((InflictedBuff)buffs[i]).GetTimeLeft() <= 0.0f)
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
