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
		if (!GameData.wentThroughMenu)
		{
			GameData.gameType = gameType;
		}

		if (GameData.gameType == GameType.DedicatedServer || GameData.gameType == GameType.ListenServer)
		{
			Network.InitializeSecurity();
			Network.InitializeServer((int)GameData.expectedConnections, 6600, Network.HavePublicAddress());
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
		else if (GameData.gamePaused)
		{
			PauseGame(false);
		}
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		_networkView.RPC("VerifyGameMode", player, (int)GameData.gameMode);

		if (Network.connections.Length == GameData.expectedConnections)
		{
			PauseGame(false);
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		if (Network.connections.Length == GameData.expectedConnections)
		{
			PauseGame(true);
		}
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{ // Just keep retrying...
		GameData.networkError = error;
		if (GameData.gameType == GameType.DedicatedServer || GameData.gameType == GameType.ListenServer)
		{ // Force NAT off maybe? So far it's the only thing that's been an issue for the server
			Network.InitializeServer((int)GameData.expectedConnections, 6600, Network.HavePublicAddress());
		}
		else if (GameData.gameType == GameType.Client)
		{
			Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600);
		}
	}

	void OnConnectedToServer()
	{ // Connected!
		GameData.networkError = NetworkConnectionError.NoError;
		if (Network.connections.Length == GameData.expectedConnections)
		{
			PauseGame(false);
		}
	}

	[RPC]
	private void VerifyGameMode(int gameMode)
	{
		if (GameData.gameMode != (GameMode)gameMode)
		{ // Whoops!
			GameData.networkError = NetworkConnectionError.IncorrectParameters; // Kind of a hack?
			Network.Disconnect();
		}
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (info == NetworkDisconnection.LostConnection)
		{ // Timed out, try to reconnect
			PauseGame(true);
			Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600);
		}
		else
		{ // Left or the server shut down... give him the option to leave
			PauseGame(true);
		}
	}

	//[RPC]
	private void PauseGame(bool state)
	{ // This does not need to be RPC because everyone should execute it when needed
		GameData.gamePaused = state;
	}
}
