using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorMenu : Editor
{
	[MenuItem("Tile generation/Generate...")]
	public static void OpenTileGenerator()
	{
		TileGenerator window = new TileGenerator();
		//TileGenerator window = (TileGenerator)ScriptableObject.CreateInstance("TileGenerator");
		window.Show();
	}
}
