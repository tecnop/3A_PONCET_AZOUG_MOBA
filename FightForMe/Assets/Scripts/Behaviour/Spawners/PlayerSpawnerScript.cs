using UnityEngine;
using System.Collections;

public class PlayerSpawnerScript : SpawnerScript
{
	[SerializeField]
	private CharacterManager _boundPlayer;

	private Vector3 _pos;

	private bool _playerDied;
	private float _playerRespawnTime;

	private static Rect centerRect;

	void Start()
	{
		_pos = this.transform.position;

		_boundPlayer.GetMiscDataScript().SetSpawner(this);

		_initialized = true;

		int w = Screen.width;
		int h = Screen.height;

		centerRect = SRect.Make(0.4f * w, 0.4f * h, 0.2f * w, 0.2f * h);

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
			_boundPlayer.GetStatsScript().Revive();
			_playerDied = false;
		}
	}

	public override void OnSpawnedEntityDeath()
	{
		_playerDied = true;
		_playerRespawnTime = 5.0f;
	}

	void Update()
	{
		if (_playerDied)
		{
			if (!GameData.gamePaused)
			{
				_playerRespawnTime -= Time.deltaTime;
				if (_playerRespawnTime < 0.0f)
				{
					Spawn();
				}
			}
		}
	}

	void OnGUI()
	{
		if (_playerDied)
		{
			if (!GameData.gamePaused && HUDRenderer.GetState() == HUDState.Default)
			{
				GUI.Box(centerRect, GUIContent.none);
				GUI.Label(centerRect, "Réapparition dans " + (_playerRespawnTime < 1.0f ? _playerRespawnTime : Mathf.FloorToInt(_playerRespawnTime)) + "s");
			}
		}
	}
}
