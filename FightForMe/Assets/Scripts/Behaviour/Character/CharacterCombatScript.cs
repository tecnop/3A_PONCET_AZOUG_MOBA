using UnityEngine;
using System.Collections;

public class CharacterCombatScript : MonoBehaviour {

	[SerializeField]
	CharacterManager _manager;

	public void Damage(GameObject target, float damage, Vector3 damageDir, int damageType)
	{
		CharacterManager hisManager = target.GetComponent<CharacterManager>();
		if (hisManager)
		{
			hisManager.GetStatsScript().loseHealth(damage);
		}
	}
}
