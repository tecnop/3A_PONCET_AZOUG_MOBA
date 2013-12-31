using UnityEngine;
using System.Collections;

public class UnitTest : MonoBehaviour
{
	void Start()
	{
		DataTables.updateTables();
		Weapon w = (Weapon)DataTables.getItem(1);
		string name = w.getName();
		float damage = w.getDamage();
		Debug.Log("Oulala ! mega dégats ! Arme : " + name + ", Degats : " + damage);
	}
}
