using UnityEngine;
using System.Collections;

public class MonsterSpawnerScript : MonoBehaviour
{
	[SerializeField] // WHY CAN'T UNSIGNED INTEGERS BE SERIALIZED
	private int[] _monsterList;		// List of monsters we should spawn (index is the level of the camp)

	[SerializeField]
	private GameObject _monsterPrefab;

	private MonsterCampScript camp;	// Camp we're a part of

	private Vector3 _pos;
	private Quaternion _ang;

	public void LinkToCamp(MonsterCampScript camp)
	{
		this.camp = camp;
	}

	void Start()
	{
		if (_monsterList.Length == 0)
		{ // We don't have any bound monsters, no reason for us to be here
			Destroy(this.gameObject);
			return;
		}

		Transform transform = this.transform;

		_pos = transform.position;
		_ang = transform.rotation;

		if (!camp)
		{ // We're not linked to a trigger, we're probably just a one-time spawner then
			Spawn();
		}
	}

	public void Spawn()
	{
		if (_monsterList.Length == 0)
		{ // We don't have any bound monsters, no reason for us to be here (checking again in case dynamic stuff happens)
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

		GameObject monsterObject = (GameObject)Instantiate(_monsterPrefab, _pos, _ang);
		CharacterManager manager = monsterObject.GetComponent<CharacterManager>();
		((MonsterMiscDataScript)manager.GetMiscDataScript()).SetSpawner(this);
		// TODO: Set the data we got from the table
	}

	public void OnBoundMonsterDeath()
	{
		if (camp)
		{
			camp.OnBoundMonsterDeath();
		}
		else
		{ // Kill us or respawn him? I'll go with respawn him for now for debugging
			Spawn();
		}
	}
}
