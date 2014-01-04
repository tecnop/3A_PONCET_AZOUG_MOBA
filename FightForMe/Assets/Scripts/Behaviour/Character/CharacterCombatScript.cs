using UnityEngine;
using System.Collections;

public class CharacterCombatScript : MonoBehaviour
{
	[SerializeField]
	CharacterManager _manager;

	public void Damage(GameObject target, float damage, Vector3 damageDir, uint damageFlags)
	{
		CharacterManager hisManager = target.GetComponent<CharacterManager>();
		if (hisManager)
		{
			hisManager.GetStatsScript().loseHealth(_manager, damage);
		}
	}
}
