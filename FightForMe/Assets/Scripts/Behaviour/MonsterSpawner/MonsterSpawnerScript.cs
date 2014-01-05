using UnityEngine;
using System.Collections;

public class MonsterSpawnerScript : MonoBehaviour
{
	[SerializeField] // WHY CAN'T UNSIGNED INTEGERS BE SERIALIZED
	private int[] _monsterList;		// List of monsters we should spawn (index is the level of the camp)

	private MonsterCampScript camp;	// Camp we're a part of

	public void LinkToCamp(MonsterCampScript camp)
	{
		this.camp = camp;
	}

	void Start()
	{
		if (!camp)
		{ // We're not linked to a trigger, we're probably just a one-time spawner then
			Spawn();
			Destroy(this.gameObject);
			return;
		}

		if (_monsterList.Length == 0)
		{ // We don't have any bound monsters, no reason for us to be here
			Destroy(this.gameObject);
			return;
		}
	}

	public void Spawn()
	{
		if (_monsterList.Length == 0)
		{ // We don't have any bound monsters, no reason for us to be here
			Destroy(this.gameObject);
			return;
		}

		uint monsterID;

		if (!camp)
		{ // Just spawn our first entry
			 monsterID = (uint)_monsterList[0];
		}
		else
		{ // Check the level of the camp, and spawn our entry with that index
			if (_monsterList.Length < camp.GetLevel() + 1)
			{
				monsterID = (uint)_monsterList[_monsterList.Length-1];
			}
			else
			{
				monsterID = (uint)_monsterList[camp.GetLevel()];
			}
		}

		Monster monster = DataTables.getMonster(monsterID);

		// TODO: Spawn him
	}
}
