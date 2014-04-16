using UnityEngine;
using System.Collections;

/*
 * GameData.cs
 * 
 * Serves as an interface between the GameMaster and all the other entities
 * Only the GameMaster and NetworkManager should be allowed to write into this
 * 
 */

public enum GameType
{
	Local,			// Bot game
	Client,			// Client
	ListenServer,	// Two-in-one server and client
	DedicatedServer	// Server executable
}

public enum GameMode
{
	KillTheLord,
	RaceForGlory
}

public static class GameData
{
	public static bool gamePaused = false;
	public static CharacterManager activePlayer;
	public static CharacterManager otherPlayer;	 // Temporary... hopefully :/
	public static GameType gameType;
	public static GameMode gameMode;
	public static bool secure;

	public static bool isServer
	{ // Should we execute server-specific code?
		get
		{
			return (gameType == GameType.Local || gameType == GameType.ListenServer || gameType == GameType.DedicatedServer);
		}
	}

	public static bool isClient
	{ // Should we execute client-specific code?
		get
		{
			return (gameType == GameType.Local || gameType == GameType.ListenServer || gameType == GameType.Client);
		}
	}

	public static bool isOnline
	{ // Are we attempting to play online?
		get
		{
			return (gameType != GameType.Local);
		}
	}

	public static uint expectedConnections
	{ // How many active connections are we expecting to have at any given time?
		get
		{
			if (gameType == GameType.DedicatedServer) return 2;
			if (gameType == GameType.ListenServer || gameType == GameType.Client) return 1;
			return 0;
		}
	}

	public static bool wentThroughMenu; // Did the game go through the main menu?

	public static NetworkConnectionError networkError; // Last connection error we got
}
