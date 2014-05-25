﻿using UnityEngine;
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

public enum PauseMessage
{ // I just realized I didn't use the same case here as other enums. Whatever I'm lazy
	DEFAULT,			// Game is paused
	CLIENT_CONNECT,		// Attempting to connect to a server
	CLIENT_RECONNECT,	// Attempting to reconnect to a server after we lost connection
	CLIENT_WAITING,		// Waiting for more clients to join
	LOST_CLIENT,		// Waiting for a client to reconnect
	SERVER_INITIALIZING,// Setting up server
	SERVER_WAITING,		// Waiting for a client to connect to us
	SERVER_WAITING2,	// Waiting for a client to reconnect to us
	CLIENT_DROP,		// Server has shutdown
	CLIENT_KICK,		// Client was kicked
	SERVER_FAILURE,		// Server failed to start
	PLAYER1_VICTORY,	// Player 1 won
	PLAYER2_VICTORY,	// Player 2 won
	INCORRECT_GAMEMODE,	// Game mode doesn't match the server's
	INCORRECT_SECURITY,	// Security level doesn't match the server's
	WAITING_FOR_CONFIG	// Waiting for the server to send us his data tables
}

public static class GameData
{
	public static bool gamePaused = false;
	public static CharacterManager activePlayer;
	public static CharacterManager otherPlayer;	 // Temporary... hopefully :/
	public static GameType gameType;
	public static GameMode gameMode;
	public static bool secure; // Is the game running the default config?

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
	public static PauseMessage pauseMessage;
}
