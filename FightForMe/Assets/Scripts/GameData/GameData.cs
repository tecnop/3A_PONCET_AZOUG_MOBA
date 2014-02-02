using UnityEngine;
using System.Collections;

/*
 * GameData.cs
 * 
 * Serves as an interface between the GameMaster and all the other entities
 * Only the GameMaster should be allowed to write into this
 * 
 */

public static class GameData
{
	public static bool isServer;
}
