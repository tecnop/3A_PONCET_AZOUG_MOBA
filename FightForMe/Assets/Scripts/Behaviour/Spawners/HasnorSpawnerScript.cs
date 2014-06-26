using UnityEngine;
using System.Collections;

public class HasnorSpawnerScript : SpawnerScript
{
	// Rushed static variables for my AI
	private static CharacterManager _hasnor;
	public static CharacterManager hasnor
	{
		get
		{
			return _hasnor;
		}
	}

	[SerializeField]
	private GameObject _monsterPrefab;

	private bool _ded; // Yes

	private Vector3 _pos;
	private Quaternion _ang;

	void Start()
	{
		_pos = _transform.position;
		_ang = _transform.rotation;

		_initialized = true;

		if (GameData.isServer)
		{
			Spawn();
		}
	}

	public override CharacterManager Spawn()
	{
		GameObject hasnor;
		if (GameData.isOnline)
		{
			hasnor = (GameObject)Network.Instantiate(_monsterPrefab, _pos, _ang, 1);
		}
		else
		{
			hasnor = (GameObject)Instantiate(_monsterPrefab, _pos, _ang);
		}
		return SetUpHasnor(hasnor);
	}

	private CharacterManager SetUpHasnor(GameObject hasnor)
	{
		CharacterManager manager = hasnor.GetComponent<CharacterManager>();

		MonsterMiscDataScript misc = (MonsterMiscDataScript)manager.GetMiscDataScript();

		misc.SetSpawner(this);

		misc.SetUpFromMonster(8); // HARD CODED REFERENCE

		_hasnor = manager;

		return manager;
	}

	public override void OnSpawnedEntityDeath()
	{
		_ded = true;
		_hasnor = null;
	}

	public void TryRespawn()
	{
		if (_ded)
		{
			_ded = false;
			Spawn();
		}
	}
}
