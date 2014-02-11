using UnityEngine;
using System.Collections;

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

	private ArrayList monsterCamps;
	private float lastSpawnTime;

	void Start()
	{
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

		DataTables.updateTables();

		if (!GameData.isServer)
		{ // We won't be needed anymore
			//Destroy(this.gameObject);
			return;
		}

		GameObject[] camps = GameObject.FindGameObjectsWithTag("MonsterCamp");
		monsterCamps = new ArrayList();
		foreach (GameObject camp in camps)
		{
			monsterCamps.Add(camp.GetComponent<MonsterCampScript>());
		}
		SpawnCamps();
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
