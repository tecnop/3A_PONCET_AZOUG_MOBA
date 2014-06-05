using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private NetworkView _networkView;

	private Dictionary<NetworkPlayer, bool> loadedList; // For servers

	void Start()
	{
		if (!GameData.wentThroughMenu)
		{
			GameData.gameType = GameType.Local;
		}

		if (GameData.gameType == GameType.Local)
		{
			Destroy(this.gameObject);
			return;
		}

		loadedList = new Dictionary<NetworkPlayer, bool>();

		if (GameData.isServer && GameData.isOnline)
		{
			Network.isMessageQueueRunning = false;
		}

		GameData.pauseMessage = PauseMessage.LOADING;
		// Pause until everyone's connected
		PauseGame(true);
	}

	void OnPlayerConnected(NetworkPlayer player)
	{ // Make sure it's someone we want, also tell him he's late
		loadedList[player] = false;
		_networkView.RPC("SetGameData", player, (int)GameData.gameMode, GameData.secure, true);
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		loadedList[player] = false;

		if (Network.connections.Length == GameData.expectedConnections)
		{
			GameData.pauseMessage = PauseMessage.LOST_CLIENT;
			PauseGame(true);
		}
	}

	public void ValidateLoading()
	{
		if (GameData.isServer)
		{ // Tell them to load
			_networkView.RPC("StartGame", RPCMode.Others);
		}
		else
		{
			_networkView.RPC("OnPlayerLoaded", RPCMode.Server, Network.player);
		}
	}

	[RPC]
	private void StartGame()
	{ // Placeholder

	}

	[RPC]
	void OnPlayerLoaded(NetworkPlayer player)
	{ // Little extra utility function called after a player has connected and has fully loaded the game
		loadedList[player] = true;

		if (Network.connections.Length == GameData.expectedConnections)
		{
			int i = 0;
			while (i < Network.connections.Length)
			{
				if (!loadedList.ContainsKey(Network.connections[i]) || !loadedList[Network.connections[i]])
				{
					return;
				}
				i++;
			}

			// Everyone's ready!

			if (GameData.isServer && GameData.isOnline)
			{
				Network.isMessageQueueRunning = true;
			}

			_networkView.RPC("Begin", RPCMode.All);
		}
	}

	[RPC]
	private void Begin()
	{ // Meh, small utility
		PauseGame(false);
	}

	[RPC]
	private void SetGameData(int gameMode, bool secure, bool inProgress)
	{ // We should never receive this but if we do then make sure someone isn't cheating his way in
		if (GameData.gameMode != (GameMode)gameMode)
		{ // Whoops!
			GameData.pauseMessage = PauseMessage.INCORRECT_GAMEMODE;
			Network.Disconnect();
		}
		else if (GameData.secure != secure)
		{
			GameData.pauseMessage = PauseMessage.INCORRECT_SECURITY;
			Network.Disconnect();
		}
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (info == NetworkDisconnection.LostConnection)
		{ // Timed out, try to reconnect
			PauseGame(true);
			GameData.pauseMessage = PauseMessage.CLIENT_RECONNECT;
			Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600);
		}
		else
		{ // Left or the server shut down... give him the option to leave
			if (GameData.pauseMessage == PauseMessage.DEFAULT)
			{ // Eeeeeh seems good
				GameData.pauseMessage = PauseMessage.CLIENT_DROP;
			}
			PauseGame(true);
		}
	}

	//[RPC]
	private void PauseGame(bool state)
	{ // This does not need to be RPC because everyone should execute it when needed
		if (!state) GameData.pauseMessage = PauseMessage.DEFAULT; // Reset it
		GameData.gamePaused = state;
	}
}
