using UnityEngine;
using System.Collections;

/*
 * GameMaster.cs
 * 
 * Tracks progress of the game, handles large-scale events and manages connections
 * 
 */

public class GameMasterScript : MonoBehaviour
{
	[SerializeField]
	private bool isServer;

	[SerializeField]
	private bool isBotGame;

	[SerializeField]
	private CharacterManager player1;

	[SerializeField]
	private CharacterManager player2;

	private ArrayList monsterCamps;
	private float lastSpawnTime;

	void Start()
	{
		Application.runInBackground = true;

		if (!isBotGame)
		{
			if (isServer)
			{
				GameData.isServer = true;
				player1.MakeLocal();
				GameData.activePlayer = player1;
				Network.maxConnections = 1;
				Network.InitializeSecurity();
				Network.InitializeServer(2, 6600, true);
			}
			else
			{
				player2.MakeLocal();
				GameData.activePlayer = player2;
				Network.Connect("127.0.0.1", 6600);
			}
		}
		else
		{
			GameData.isServer = true;
			player1.MakeLocal();
			GameData.activePlayer = player1; // Temporary?
			//player2.MakeBotOrSomething();
		}

		DataTables.updateTables();

		if (!isServer)
		{ // We won't be needed anymore
			Destroy(this.gameObject);
			return;
		}

		GameObject[] camps = GameObject.FindGameObjectsWithTag("MonsterCamp");
		monsterCamps = new ArrayList();
		foreach (GameObject camp in camps)
		{
			monsterCamps.Add(camp.GetComponent<MonsterCampScript>());
		}
		SpawnCamps();

		if (!isBotGame)
		{ // Pause until the second player connects
			PauseGame(true);
		}
	}

	private void SpawnCamps()
	{
		foreach (MonsterCampScript camp in monsterCamps)
		{
			camp.TrySpawn();
		}
		lastSpawnTime = Time.time;
	}

	void Update()
	{
		if (Time.time - lastSpawnTime > 120.0f)
		{ // There is probably a better way to run such a simple timer...
			SpawnCamps();
		}

		if (GameData.gamePaused)
		{ // Make the timer catch up
			lastSpawnTime += Time.deltaTime;
		}
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		if (Network.connections.Length == 1)
		{
			Debug.Log("Unpausing game");
			PauseGame(false);
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		if (Network.connections.Length < 2)
		{
			Debug.Log("Pausing game");
			PauseGame(true);
		}
	}

	[RPC]
	private static void PauseGame(bool state)
	{ // This does not need to be RPC for now considering it will only be called if the client is MISSING, but I have plans...
		GameData.gamePaused = state;
	}
}
