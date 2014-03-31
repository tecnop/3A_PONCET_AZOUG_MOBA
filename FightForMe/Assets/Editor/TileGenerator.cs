using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileGenerator : EditorWindow
{
	[SerializeField]
	private GameObject tilePrefab;

	[SerializeField]
	private GameObject terrain;

	[SerializeField]
	private GameObject tiles;

	void OnGUI()
	{
		GameObject temp;
		
		temp = (GameObject)EditorGUILayout.ObjectField("Tile prefab", this.tilePrefab, typeof(GameObject));

		if (temp != null && PrefabUtility.GetPrefabType(temp) == PrefabType.Prefab)
		{
			this.tilePrefab = temp;
		}

		temp = (GameObject)EditorGUILayout.ObjectField("Terrain entity", this.terrain, typeof(GameObject));

		if (temp != null && PrefabUtility.GetPrefabType(temp) != PrefabType.Prefab)
		{
			this.terrain = temp;
		}

		temp = (GameObject)EditorGUILayout.ObjectField("Tiles parent", this.tiles, typeof(GameObject));

		if (temp != null && PrefabUtility.GetPrefabType(temp) != PrefabType.Prefab)
		{
			this.tiles = temp;
		}

		if (this.tilePrefab != null && this.terrain != null && this.tiles != null &&
			GUILayout.Button("Generate"))
		{
			DoGenerate();
		}
	}

	private void DoGenerate()
	{
		// Create a table as large as the terrain
		// Spawn tiles everywhere on the terrain and store them in the table
		// Once that's done, double parse the table to link every tile to the tiles it can see
	}
}
