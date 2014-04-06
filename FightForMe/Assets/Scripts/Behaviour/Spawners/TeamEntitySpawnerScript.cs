using UnityEngine;
using System.Collections;

public enum TeamEntity
{
	Player,
	Minion,
	Lord
}

public class TeamEntitySpawnerScript : SpawnerScript
{ // NOTE: Unused for now
	[SerializeField]
	private TeamEntity _entityType;			// Which type of team entity we're spawning

	[SerializeField]
	private GameObject _monsterPrefab;		// The monster prefab in case we're a lord/minion spawner

	[SerializeField]
	private CharacterManager _playerManager;	// If we're a player spawner, this is the player we're spawning

	private bool playerIsDead;

	//static Monster minion = new Monster("Minion", AIType.roaming);

	void Start()
	{
		if (!GameData.isServer && _entityType != TeamEntity.Player)
		{ // We're a server-only entity
			Destroy(this.gameObject);
			return;
		}

		playerIsDead = false;
	}

	public override void OnSpawnedEntityDeath()
	{
		if (_entityType == TeamEntity.Lord)
		{ // The Lord is dead!
			// Here: Victory sequence
			Debug.Log("Lord died!");
		}
		else if (_entityType == TeamEntity.Player)
		{ // Respawn him after some time
			
		}
	}

	public override void Spawn()
	{
		if (playerIsDead)
		{
		}
	}
}
