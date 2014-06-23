using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawnerScript : SpawnerScript
{
	[SerializeField]
	private CharacterManager _boundPlayer;

	private Vector3 _pos;

	private bool _playerDied;
	private float _playerRespawnTime;

	private static Rect centerRect;

	private static List<DamageInstance> log;

	private static Vector2 scrollPos;

	void Start()
	{
		_pos = this.transform.position;

		_boundPlayer.GetMiscDataScript().SetSpawner(this);

		_initialized = true;

		int w = Screen.width;
		int h = Screen.height;

		centerRect = SRect.Make(0.35f * w, 0.4f * h, 0.3f * w, 0.2f * h);

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
				string respawnTime = "Réapparition dans " + (_playerRespawnTime < 1.0f ? Mathf.Round(_playerRespawnTime * 10.0f) / 10.0f : Mathf.FloorToInt(_playerRespawnTime)) + "s";

				GUI.Box(centerRect, GUIContent.none);
				GUI.BeginGroup(centerRect);
				if (log == null || log.Count == 0)
				{
					GUI.Label(centerRect, respawnTime, FFMStyles.centeredText_wrapped);
				}
				else
				{
					GUI.Label(SRect.Make(0.0f, 0.0f, centerRect.width, 20.0f), respawnTime, FFMStyles.centeredText_wrapped);

					float scrollHeight = 20.0f * log.Count;
					if (scrollHeight < centerRect.height - 20.0f) scrollHeight = centerRect.height - 20.0f;

					scrollPos = GUI.BeginScrollView(SRect.Make(2.0f, 20.0f, centerRect.width, centerRect.height - 20.0f), scrollPos, SRect.Make(0.0f, 20.0f, centerRect.width - 20.0f, scrollHeight), false, true);
					for (int i = 0; i < log.Count; i++)
					{
						GUI.Label(SRect.Make(0.0f, 20.0f * (i+1), centerRect.width - 20.0f, 20.0f), log[i].ToString());
					}
					GUI.EndScrollView(true);
				}
				GUI.EndGroup();
			}
		}
	}

	public void SetLog(List<DamageInstance> log)
	{
		List<DamageInstance> actualLog = new List<DamageInstance>(log.Count);

		foreach (DamageInstance entry in log)
		{
			if (!entry.selfCast)
			{
				actualLog.Add(entry);
			}
		}

		PlayerSpawnerScript.log = actualLog;
	}
}
