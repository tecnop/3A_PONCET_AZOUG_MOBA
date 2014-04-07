using UnityEngine;
using System.Collections;

public class HasnorSpawnerScript : SpawnerScript
{
	[SerializeField]
	private GameObject _monsterPrefab;

	private bool _ded; // Yes

	private Vector3 _pos;
	private Quaternion _ang;

	void Start()
	{
		Transform transform = this.transform;
		_pos = transform.position;
		_ang = transform.rotation;

		_initialized = true;

		if (GameData.isServer)
		{
			Spawn();
		}
	}

	public override void Spawn()
	{
		GameObject hasnor;
		if (GameData.isOnline)
		{
			hasnor = (GameObject)Network.Instantiate(_monsterPrefab, _pos, _ang, 0);
		}
		else
		{
			hasnor = (GameObject)Instantiate(_monsterPrefab, _pos, _ang);
		}
		SetUpHasnor(hasnor);
	}

	private void SetUpHasnor(GameObject hasnor)
	{
		CharacterManager manager = hasnor.GetComponent<CharacterManager>();

		MonsterMiscDataScript misc = (MonsterMiscDataScript)manager.GetMiscDataScript();

		misc.SetSpawner(this);

		misc.SetUpFromMonster(8); // HARD CODED REFERENCE
	}

	public override void OnSpawnedEntityDeath()
	{
		_ded = true;
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
