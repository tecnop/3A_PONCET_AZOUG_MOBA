using UnityEngine;
using System.Collections;

public class InstantiateManagerScript : MonoBehaviour
{
	[SerializeField]
	private GameObject monsterPrefab;

	[SerializeField]
	private GameObject projectilePrefab;

	void Start()
	{ // Do stuff

	}

	private GameObject SpawnEntity(GameObject prefab, Vector3 pos, Quaternion ang)
	{
		if (GameData.isOnline)
		{
			return (GameObject)Network.Instantiate(prefab, pos, ang, 0);
		}
		else
		{
			return (GameObject)Instantiate(prefab, pos, ang);
		}
	}

	public CharacterManager SpawnMonster(uint monsterID, Vector3 pos, Quaternion ang)
	{
		/*// Spawn the entity
		GameObject monsterObject = SpawnEntity(monsterPrefab, pos, ang);

		// Link us together
		CharacterManager manager = monsterObject.GetComponent<CharacterManager>();
		MonsterMiscDataScript misc = (MonsterMiscDataScript)manager.GetMiscDataScript();
		misc.SetSpawner(this);

		// Set him up
		misc.SetUpFromMonster(monsterID);*/
		return null;
	}
}
