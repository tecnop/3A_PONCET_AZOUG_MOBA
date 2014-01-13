using UnityEngine;
using System.Collections;

public class CharacterCombatScript : MonoBehaviour
{
	[SerializeField]
	CharacterManager _manager;

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
}
