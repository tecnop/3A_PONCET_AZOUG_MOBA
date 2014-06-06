using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * GameMaster.cs
 * 
 * Initializes the game and handles large-scale events
 * 
 */

public class GameMasterScript : MonoBehaviour
{
	[SerializeField]
	private NetworkView _networkView;

	[SerializeField]
	private CharacterManager player1;

	[SerializeField]
	private CharacterManager player2;

	[SerializeField]
	private GameMode gameMode;

	private List<MonsterCampScript> monsterCamps;
	private float nextSpawnTime;

	void Start()
	{
		if (!GameData.wentThroughMenu)
		{
			GameData.gameMode = gameMode;
			GameData.secure = true;
		}

		if (GameData.gameType == GameType.DedicatedServer)
		{ // TODO
			//_networkView.RPC("LinkMeToPlayer", Network.connections[0], true);
			//_networkView.RPC("LinkMeToPlayer", Network.connections[1], false);
		}
		else if (GameData.gameType == GameType.ListenServer)
		{
			LinkMeToPlayer(true);
			//_networkView.RPC("LinkMeToPlayer", Network.connections[0], false);
		}
		else if (GameData.gameType == GameType.Local)
		{
			LinkMeToPlayer(true);
			// Make the other one a bot
		}
		else
		{ // We'll just receive it from the network
			LinkMeToPlayer(false); // TEMPORARY
		}

		if (!GameData.wentThroughMenu)
		{ // Do it now
			DataTables.FillTables();
		}

		if (!GameData.isServer)
		{ // We won't be needed anymore
			//Destroy(this.gameObject);
			return;
		}

		GameObject[] camps = GameObject.FindGameObjectsWithTag("MonsterCamp");
		monsterCamps = new List<MonsterCampScript>();
		foreach (GameObject camp in camps)
		{
			monsterCamps.Add(camp.GetComponent<MonsterCampScript>());
		}

		// Wait 3 seconds to make sure everyone's in the game and ready
		nextSpawnTime = 3.0f;
	}

	private void SpawnCamps()
	{
		foreach (MonsterCampScript camp in monsterCamps)
		{
			camp.TrySpawn();
		}
		nextSpawnTime = 120.0f;
	}

	void Update()
	{
		if (GameData.isServer && !GameData.gamePaused)
		{
			nextSpawnTime -= Time.deltaTime;

			if (nextSpawnTime <= 0.0f)
			{
				SpawnCamps();
			}
		}
	}

	[RPC]
	private void LinkMeToPlayer(bool first)
	{
		if (first)
		{
			player1.MakeLocal();
			GameData.activePlayer = player1;
			GameData.otherPlayer = player2;
		}
		else
		{
			player2.MakeLocal();
			GameData.activePlayer = player2;
			GameData.otherPlayer = player1;
		}
	}
}
