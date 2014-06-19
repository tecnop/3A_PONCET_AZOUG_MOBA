using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * NetworkScript.cs
 * 
 * Ensures clients properly join the server after they exit the lobby and handles reconnecting to games
 * Also handles pausing the game while players are reconnecting
 * 
 */

public class NetworkScript : MonoBehaviour
{
	[SerializeField]
	private NetworkView _networkView;

	private Dictionary<NetworkPlayer, bool> loadedList; // For servers

	private bool ready; // For the client

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

		Network.SetLevelPrefix(1);

		// WARNING: YOU ARE ABOUT TO WITNESS SOMETHING TERRIBLE









		// YOU HAVE BEEN WARNED









		if (GameData.isOnline)
		{ // Okay, here it comes. Please read the comment before judging me :<
			if (!GameData.isServer)
			{ // So there. Because upon connecting the server sends us a bunch of useless data when we're not even in the right scene,
			  // we disconnect after loading and reconnect to re-receive all the buffered RPCs.
				ready = false;
				Network.Disconnect();
				Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600);
				// So, how was it? Are you still alive? Hello?
			}
		}

		GameData.pauseMessage = PauseMessage.LOADING;
		// Pause until everyone's connected
		PauseGame(true);
	}

	void OnPlayerConnected(NetworkPlayer player)
	{ // Make sure it's someone we want, also tell him he's late
		loadedList[player] = false;
		_networkView.RPC("SetGameData", player, (int)GameData.gameMode, GameData.secure, true);

		if (!GameData.secure)
		{
			_networkView.RPC("SetDataTablesConfig", player, DataTables.GetConfigString());
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		loadedList[player] = false;

		//Network.SetSendingEnabled(player, 1, false);

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
			if (Network.connections.Length > 0)
			{
				_networkView.RPC("OnPlayerLoaded", RPCMode.Server, Network.player);
			}
			else
			{
				ready = true;
			}
		}
	}

	[RPC]
	private void StartGame()
	{ // Placeholder so that we may redirect this to the LobbyScript

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

	[RPC]
	private void SetDataTablesConfig(string configString)
	{ // Placerholder, should never be received here

	}

	void OnConnectedToServer()
	{ // Connected!
		GameData.networkError = NetworkConnectionError.NoError;

		if (ready)
		{ // Go for it again
			ValidateLoading();
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
