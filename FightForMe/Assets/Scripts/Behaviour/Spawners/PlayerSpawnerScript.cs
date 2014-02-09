using UnityEngine;
using System.Collections;

public class PlayerSpawnerScript : SpawnerScript
{
	[SerializeField]
	private CharacterManager _boundPlayer;

	private Vector3 _pos;

	private bool _playerDied;
	private float _playerRespawnTime;

	void Start()
	{
		_pos = this.transform.position;

		_boundPlayer.GetMiscDataScript().SetSpawner(this);

		_initialized = true;

		//if (_spawnPending)
		{
			Spawn();
		}
	}

	public override void Spawn()
	{
		if (!_initialized)
		{
			_spawnPending = true;
			return;
		}

		_boundPlayer.GetCharacterTransform().position = _pos;
		if (_playerDied)
		{ // Make him alive again
			_boundPlayer.GetCharacterAnimator().SetBool("isDead", false);
			_playerDied = false;
		}
	}

	public override void OnSpawnedEntityDeath()
	{
		_playerDied = true;
		_playerRespawnTime = Time.time + 5.0f;
	}

	void Update()
	{
		if (_playerDied)
		{
			if (GameData.gamePaused)
			{
				_playerRespawnTime += Time.deltaTime;
			}
			else if (Time.time > _playerRespawnTime)
			{
				Spawn();
			}
		}
	}
}
