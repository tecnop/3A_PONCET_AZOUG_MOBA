using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorMenu : Editor
{
	[MenuItem("Map generation/Generate tiles")]
	public static void OpenTileGenerator()
	{
		TileGenerator.Open();
	}

	[MenuItem("Map generation/Paintbrush")]
	public static void OpenPaintBrush()
	{
		PaintBrush.Open();
	}
}
