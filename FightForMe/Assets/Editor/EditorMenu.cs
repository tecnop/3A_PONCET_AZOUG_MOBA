using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorMenu : Editor
{
	[MenuItem("Tile generation/Generate...")]
	public static void OpenTileGenerator()
	{
		TileGenerator.Open();
	}
}
