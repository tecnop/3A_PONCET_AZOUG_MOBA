﻿using UnityEngine;
using System.Collections;

/*
 * NetworkScript.cs
 * 
 * Handles server initialization and client connections
 * Also handles pausing
 * 
 */

public class NetworkScript : MonoBehaviour
{
	[SerializeField]
	private GameType gameType;

	[SerializeField]
	private NetworkView _networkView;

	void Start()
	{
		Application.runInBackground = true;

		if (!GameData.wentThroughMenu)
		{
			GameData.gameType = gameType;
		}

		if (GameData.gameType == GameType.DedicatedServer || GameData.gameType == GameType.ListenServer)
		{
			Network.InitializeSecurity();
			Network.InitializeServer((int)GameData.expectedConnections, 6600, true);
		}
		else if (GameData.gameType == GameType.Client)
		{
			Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600); // 127.0.0.1
		}
		else
		{ // No network... delete us?
		}

		if (GameData.gameType != GameType.Local)
		{ // Pause until everyone's connected
			PauseGame(true);
		}
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		if (Network.connections.Length == GameData.expectedConnections)
		{
			Debug.Log("Unpausing game");
			PauseGame(false);
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		if (Network.connections.Length < GameData.expectedConnections)
		{
			Debug.Log("Pausing game");
			PauseGame(true);
		}
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{ // Just keep retrying...
		Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600);
	}

	//[RPC]
	private void PauseGame(bool state)
	{ // This does not need to be RPC because everyone should execute it at once
		GameData.gamePaused = state;
	}
}
