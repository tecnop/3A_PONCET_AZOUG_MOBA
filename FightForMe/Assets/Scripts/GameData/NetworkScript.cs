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

	private bool isLoaded; // For clients

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

		isLoaded = false;

		loadedList = new Dictionary<NetworkPlayer, bool>();

		GameData.pauseMessage = PauseMessage.LOADING;
		// Pause until everyone's connected
		PauseGame(true);
	}

	void OnPlayerConnected(NetworkPlayer player)
	{ // Make sure it's someone we want, also tell him he's late
		loadedList[player] = false;
		_networkView.RPC("VerifyGameData", player, (int)GameData.gameMode, GameData.secure);
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

		}
		else
		{
			_networkView.RPC("OnPlayerLoaded", RPCMode.Server, Network.player);
		}
	}

	[RPC]
	void OnPlayerLoaded(NetworkPlayer player)
	{ // Little extra utility function called after a player has connected and has fully loaded the game
		if (Network.connections.Length == GameData.expectedConnections)
		{
			int i = 0;
			while (i < Network.connections.Length)
			{
				if (!loadedList[Network.connections[i]])
				{
					return;
				}
				i++;
			}

			// Everyone's ready!
			PauseGame(false);
		}
	}

	[RPC]
	private void VerifyGameData(int gameMode, bool secure)
	{
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
	{
		if (!GameData.secure)
		{ // Small security measure
			DataTables.SetConfigString(configString);

			if (Network.connections.Length == GameData.expectedConnections)
			{ // We're ready
				PauseGame(false);
			}
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
