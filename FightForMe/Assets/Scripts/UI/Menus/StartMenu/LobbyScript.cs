using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyScript : MonoBehaviour
{
	Rect screenRect;
	GUIStyle text;

	LobbyMessage lobbyMessage;

	bool ready;
	bool started;

	[SerializeField]
	private NetworkView _networkView;

	[SerializeField]
	private string _mode1Scene;

	[SerializeField]
	private string _mode2Scene;

	[SerializeField]
	private string _tutorialScene;

	private Dictionary<NetworkPlayer, bool> readyList;

	void Start()
	{
		screenRect = new Rect(0.0f, 0.0f, Screen.width, Screen.height);

		text = new GUIStyle();
		text.alignment = TextAnchor.MiddleCenter;
		text.wordWrap = true;
		text.normal.textColor = Color.white;

		ready = false;
		started = false;

		readyList = new Dictionary<NetworkPlayer, bool>();

		if (GameData.isServer)
		{ // Fill the tables now so we can send them
			DataTables.FillTables();

			if (DataTables.GetError() != null)
			{ // OnGUI will do the rest
				return;
			}
		}

		Network.SetLevelPrefix(0);

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

		if (GameData.gameType == GameType.Local)
		{
			GUI.Label(screenRect, "Chargement en cours...", text);
		}
		else if (DataTables.GetError() != null)
		{
			GUI.Label(screenRect, DataTables.GetError().Message, text);
		}
		else if (lobbyMessage == LobbyMessage.DATA_ERROR)
		{
			GUI.Label(screenRect, "Le server a envoyé un fichier de configuration mal formé", text);
		}
		else if (lobbyMessage == LobbyMessage.GAME_DATA
			|| lobbyMessage == LobbyMessage.READY
			|| lobbyMessage == LobbyMessage.CLIENT_WAITING
			|| lobbyMessage == LobbyMessage.GAME_STARTED)
		{
			GUI.Label(new Rect(0.25f * w, 0.25f * h, 0.5f * w, 30), "Partie " + (GameData.secure ? "sécurisée" : "non sécurisée"), text);
			GUI.Label(new Rect(0.25f * w, 0.25f * h + 30, 0.5f * w, 30), "Mode de jeu: " + (GameData.gameMode == GameMode.KillTheLord ? "Suprématie" : "Course à la Gloire"), text);
			if (Network.connections.Length > 0)
			{
				GUI.Label(new Rect(0.25f * w, 0.25f * h + 60, 0.5f * w, 30), "Ping: " + Network.GetAveragePing(Network.connections[0]), text);
			}

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
				else
				{
					if (lobbyMessage == LobbyMessage.GAME_STARTED)
					{ // We're late to the party
						if (GUI.Button(new Rect(0.25f * w, 0.25f * h + 90, 0.5f * w, 30), "Se reconnecter à la partie"))
						{
							StartGame();
						}
					}
					else
					{
						if (GUI.Button(new Rect(0.25f * w, 0.25f * h + 90, 0.5f * w, 30), "Commencer!"))
						{
							lobbyMessage = LobbyMessage.READY;
							if (GameData.isServer)
							{
								ready = true;

								if (EveryoneReady())
								{
									StartGame();
								}
							}
							else
							{
								_networkView.RPC("SetReady", RPCMode.Server, Network.player);
							}
						}
						else if (GameData.isServer && GUI.Button(new Rect(0.25f * w, 0.25f * h + 120, 0.5f * w, 30), "Exclure l'autre joueur"))
						{
							_networkView.RPC("DoKick", Network.connections[0]);
							//Network.CloseConnection(Network.connections[0], true);
						}
					}
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
				GUI.Label(screenRect, lobbyMessage.ToString() + "???", text);
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
		readyList.Remove(player);
		foreach (NetworkPlayer key in readyList.Keys)
		{ // Lost a player, noone should be ready anymore
			readyList[key] = false;
		}
		ready = false;

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

		if (GameData.isClient)
		{ // Yes, I know, it sounds dumb
			lobbyMessage = LobbyMessage.CLIENT_WAITING;
		}
		else
		{
			lobbyMessage = LobbyMessage.SERVER_WAITING;
		}
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
		if (inProgress && !secure)
		{
			lobbyMessage = LobbyMessage.GAME_STARTED;
		}

		if (secure)
		{ // Great, just load our default tables
			DataTables.FillTables();
		}
	}

	[RPC]
	private void SetDataTablesConfig(string configString)
	{
		if (!GameData.secure)
		{ // Small security measure
			try
			{
				DataTables.SetConfigString(configString);
			}
			catch (System.Exception e)
			{ // That shouldn't have happened! If it ever does, then that means the server willingly sent us something bad, so get out right now
				Debug.LogError(e.Message);
				lobbyMessage = LobbyMessage.DATA_ERROR;
				Network.Disconnect();
				return;
			}

			if (started)
			{
				lobbyMessage = LobbyMessage.GAME_STARTED;
			}
			else if (Network.connections.Length == GameData.expectedConnections)
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

		if (EveryoneReady())
		{
			StartGame();
		}
	}

	private bool EveryoneReady()
	{
		if (Network.connections.Length == GameData.expectedConnections)
		{ // Little security check
			int i = 0;
			while (i < Network.connections.Length)
			{
				if (!readyList.ContainsKey(Network.connections[i]) || !readyList[Network.connections[i]])
				{
					return false;
				}
				i++;
			}

			if (!GameData.isClient || ready)
			{ // Everyone's ready!
				return true;
			}
		}
		return false;
	}

	[RPC]
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
		else if (GameData.gameMode == GameMode.Tutorial)
		{
			Application.LoadLevel(_tutorialScene);
		}
	}

	[RPC]
	private void DoKick()
	{
		lobbyMessage = LobbyMessage.CLIENT_KICK;
		Network.Disconnect();
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (info == NetworkDisconnection.LostConnection)
		{ // Timed out, try to reconnect
			lobbyMessage = LobbyMessage.CLIENT_RECONNECT;
			Network.Connect(PlayerPrefs.GetString("ipAddress"), 6600); // This is not the best, if another client is executed and changes the playerPrefs, someone can be rerouted to another server
		}
		else if (lobbyMessage != LobbyMessage.CLIENT_KICK && lobbyMessage != LobbyMessage.DATA_ERROR)
		{ // Left or the server shut down
			lobbyMessage = LobbyMessage.CLIENT_DROP;
		}
	}
}
