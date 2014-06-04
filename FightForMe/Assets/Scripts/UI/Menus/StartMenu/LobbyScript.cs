﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyScript : MonoBehaviour
{
	Rect screenRect;
	GUIStyle text;

	LobbyMessage lobbyMessage;

	bool started;

	[SerializeField]
	private NetworkView _networkView;

	[SerializeField]
	private string _mode1Scene;

	[SerializeField]
	private string _mode2Scene;

	private Dictionary<NetworkPlayer, bool> readyList;

	void Start()
	{
		screenRect = new Rect(0.0f, 0.0f, Screen.width, Screen.height);

		text = new GUIStyle();
		text.alignment = TextAnchor.MiddleCenter;
		text.wordWrap = true;
		text.normal.textColor = Color.white;

		started = false;

		readyList = new Dictionary<NetworkPlayer, bool>();

		if (GameData.gameType == GameType.DedicatedServer || GameData.gameType == GameType.ListenServer)
		{
			lobbyMessage = LobbyMessage.SERVER_INITIALIZING;
			Network.InitializeSecurity();
			Network.InitializeServer((int)GameData.expectedConnections, 6600, !Network.HavePublicAddress());
		}
		else if (GameData.gameType == GameType.Client)
		{
			lobbyMessage = LobbyMessage.CLIENT_CONNECT;
			Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600);
		}
		else
		{ // No network, just skip right ahead
			StartGame();
		}
	}

	void OnGUI()
	{
		int w = Screen.width;
		int h = Screen.height;

		if (lobbyMessage == LobbyMessage.GAME_DATA || lobbyMessage == LobbyMessage.READY || lobbyMessage == LobbyMessage.CLIENT_WAITING)
		{
			GUI.Label(new Rect(0.25f * w, 0.25f * h, 0.5f * w, 30), "Partie " + (GameData.secure ? "sécurisée" : "non sécurisée"), text);
			GUI.Label(new Rect(0.25f * w, 0.25f * h + 30, 0.5f * w, 30), "Mode de jeu: " + (GameData.gameMode == GameMode.KillTheLord ? "Suprématie" : "Course à la Gloire"), text);
			GUI.Label(new Rect(0.25f * w, 0.25f * h + 60, 0.5f * w, 30), "Ping: " + Network.GetAveragePing(Network.connections[0]), text);

			if (lobbyMessage == LobbyMessage.CLIENT_WAITING)
			{
				GUI.Label(new Rect(0.25f * w, 0.25f * h + 90, 0.5f * w, 30), "Attente de la connexion d'autres joueurs...", text);
			}
			else
			{
				if (lobbyMessage == LobbyMessage.READY)
				{
					GUI.Label(new Rect(0.25f * w, 0.25f * h + 90, 0.5f * w, 30), "En attente que les autres joueurs acceptent la partie...", text);
				}
				else if (GUI.Button(new Rect(0.25f * w, 0.25f * h + 90, 0.5f * w, 30), "Commencer!"))
				{
					lobbyMessage = LobbyMessage.READY;
					_networkView.RPC("SetReady", RPCMode.Server, Network.player);
				}
			}
		}
		else
		{
			if (lobbyMessage == LobbyMessage.CLIENT_CONNECT)
			{
				GUI.Label(screenRect, "Connexion en cours...", text);
			}
			else if (lobbyMessage == LobbyMessage.CLIENT_RECONNECT)
			{
				GUI.Label(screenRect, "Reconnexion en cours...", text);
			}
			else if (lobbyMessage == LobbyMessage.SERVER_INITIALIZING)
			{
				GUI.Label(screenRect, "Mise en place du serveur...", text);
			}
			else if (lobbyMessage == LobbyMessage.SERVER_WAITING)
			{
				GUI.Label(screenRect, "Attente de connexion des joueurs...", text);
			}
			else if (lobbyMessage == LobbyMessage.CLIENT_KICK)
			{
				GUI.Label(screenRect, "Vous avez été exclu(e) de la partie.", text);
			}
			else if (lobbyMessage == LobbyMessage.CLIENT_DROP)
			{
				GUI.Label(screenRect, "Le serveur s'est deconnecté.", text);
			}
			else if (lobbyMessage == LobbyMessage.SERVER_FAILURE)
			{
				GUI.Label(screenRect, "Echec de la mise en place du serveur.", text);
			}
			else if (lobbyMessage == LobbyMessage.WAITING_FOR_DATA)
			{
				GUI.Label(screenRect, "Téléchargement du fichier de configuration du serveur...", text);
			}
			else
			{
				GUI.Label(screenRect, "???" + lobbyMessage.ToString(), text);
			}
		}

		if (GUI.Button(new Rect(0.4f * w, 0.8f * h, 0.2f * w, 0.1f * h), "Quitter"))
		{
			Network.Disconnect();
			Application.LoadLevel("startMenu");
		}
	}

	void OnPlayerConnected(NetworkPlayer player)
	{ // Send him the current game's data so he knows what to expect
		if (readyList.ContainsKey(player))
		{
			readyList[player] = false;
		}
		else
		{
			readyList.Add(player, false);
		}
		_networkView.RPC("SetGameData", player, (int)GameData.gameMode, GameData.secure, false);

		if (!GameData.secure)
		{
			_networkView.RPC("SetDataTablesConfig", player, DataTables.GetConfigString());
		}

		if (Network.connections.Length == GameData.expectedConnections)
		{
			lobbyMessage = LobbyMessage.GAME_DATA;
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		readyList[player] = false;
		if (Network.connections.Length == GameData.expectedConnections)
		{
			if (GameData.isClient)
			{
				lobbyMessage = LobbyMessage.CLIENT_WAITING;
			}
			else
			{
				lobbyMessage = LobbyMessage.SERVER_WAITING;
			}
		}
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{ // Just keep retrying...
		GameData.networkError = error;
		if (GameData.gameType == GameType.DedicatedServer || GameData.gameType == GameType.ListenServer)
		{
			lobbyMessage = LobbyMessage.SERVER_FAILURE;

			// DEBUG: Force NAT off so I can work offline
			Network.InitializeServer((int)GameData.expectedConnections, 6600, false);
		}
		else if (GameData.gameType == GameType.Client)
		{
			Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600);
		}
	}

	void OnServerInitialized()
	{ // Make sure it resets the display if an error occured before
		GameData.networkError = NetworkConnectionError.NoError;
		lobbyMessage = LobbyMessage.SERVER_WAITING;
	}

	void OnConnectedToServer()
	{ // Connected!
		GameData.networkError = NetworkConnectionError.NoError;
		if (GameData.secure)
		{ // Connect instantly
			if (Network.connections.Length == GameData.expectedConnections)
			{
				lobbyMessage = LobbyMessage.GAME_DATA;
			}
			else
			{
				lobbyMessage = LobbyMessage.CLIENT_WAITING;
			}
		}
		else
		{ // We want to wait for the data tables
			lobbyMessage = LobbyMessage.WAITING_FOR_DATA;
		}
	}

	[RPC]
	private void SetGameData(int gameMode, bool secure, bool inProgress)
	{
		GameData.gameMode = (GameMode)gameMode;
		GameData.secure = secure;
		started = inProgress;
	}

	[RPC]
	private void SetDataTablesConfig(string configString)
	{
		if (!GameData.secure)
		{ // Small security measure
			DataTables.SetConfigString(configString);

			if (Network.connections.Length == GameData.expectedConnections)
			{
				lobbyMessage = LobbyMessage.GAME_DATA;
			}
			else
			{
				lobbyMessage = LobbyMessage.CLIENT_WAITING;
			}
		}
	}

	[RPC]
	private void SetReady(NetworkPlayer player)
	{
		readyList[player] = true;

		if (Network.connections.Length == GameData.expectedConnections)
		{ // Little security check
			int i = 0;
			while (i < Network.connections.Length)
			{
				if (!readyList[Network.connections[i]])
				{
					return;
				}
				i++;
			}

			// Everyone's ready!
			StartGame();
		}
	}

	private void StartGame()
	{
		if (GameData.gameMode == GameMode.KillTheLord)
		{
			Application.LoadLevel(_mode1Scene);
		}
		else if (GameData.gameMode == GameMode.RaceForGlory)
		{
			Application.LoadLevel(_mode2Scene);
		}
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (info == NetworkDisconnection.LostConnection)
		{ // Timed out, try to reconnect
			lobbyMessage = LobbyMessage.CLIENT_RECONNECT;
			Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600); // This is not the best, if another client is executed and changes the playerPrefs, someone can be rerouted to another server
		}
		{ // Left or the server shut down
			lobbyMessage = LobbyMessage.CLIENT_DROP;
		}
	}
}
