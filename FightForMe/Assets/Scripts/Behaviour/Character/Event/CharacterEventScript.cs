using UnityEngine;
using System.Collections;

public abstract class CharacterEventScript : MonoBehaviour {

	[SerializeField]
	CharacterManager _manager;

	public abstract void OnPain(float damage);
	public abstract void OnDeath(GameObject killer);
	public abstract void OnSpotEnemy(GameObject enemy);
}
