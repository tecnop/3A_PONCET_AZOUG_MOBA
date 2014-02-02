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
	private bool botGame;

	[SerializeField]
	private CharacterManager player1;

	[SerializeField]
	private CharacterManager player2;

	void Start()
	{
		Application.runInBackground = true;

		if (isServer)
		{
			player1.MakeLocal();
			Network.InitializeSecurity();
			Network.InitializeServer(2, 6600, true);
		}
		else
		{
			player2.MakeLocal();
			Network.Connect("127.0.0.1", 6600);
		}

		DataTables.updateTables();
	}
}
