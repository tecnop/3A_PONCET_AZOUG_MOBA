using UnityEngine;
using System.Collections;

public class CharacterCombatScript : MonoBehaviour
{
	private CharacterManager _manager;

	private ArrayList buffs;		// List of active buffs and debuffs (type: InflictedBuff)

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
	}

	public void Damage(GameObject target, float damage, Vector3 damageDir = new Vector3(), uint damageFlags = 0)
	{
		CharacterManager hisManager = target.GetComponent<CharacterManager>();
		if (hisManager)
		{
			this.Damage(hisManager, damage, damageDir, damageFlags);
		}
	}

	public void RecieveBuff(CharacterManager inflictor, uint buffID, float duration)
	{
		this.buffs.Add(new InflictedBuff(buffID, duration, inflictor));
		_manager.GetStatsScript().UpdateStats();
	}

	public void InflictBuff(CharacterManager target, uint buffID, float duration)
	{
		target.GetCombatScript().RecieveBuff(_manager, buffID, duration);
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
			else
			{
				// TODO: Check periodic effects like DoT
			}
		}

		if (updated)
		{
			_manager.GetStatsScript().UpdateStats();
		}
	}
}
