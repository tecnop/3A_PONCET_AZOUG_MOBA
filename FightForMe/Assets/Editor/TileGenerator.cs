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
		
		temp = (GameObject)EditorGUILayout.ObjectField("Tile prefab", this.tilePrefab, typeof(GameObject), false);

		if (temp != null && temp.GetComponent<MapTileScript>() != null)
		{
			this.tilePrefab = temp;
		}

		temp = (GameObject)EditorGUILayout.ObjectField("Terrain entity", this.terrain, typeof(GameObject), true);

		if (temp != null && PrefabUtility.GetPrefabType(temp) != PrefabType.Prefab)
		{
			this.terrain = temp;
		}

		temp = (GameObject)EditorGUILayout.ObjectField("Tiles parent", this.tiles, typeof(GameObject), true);

		if (temp != null && PrefabUtility.GetPrefabType(temp) != PrefabType.Prefab)
		{
			this.tiles = temp;
		}

		GUILayout.Space(10);

		if (this.tilePrefab != null && this.terrain != null && this.tiles != null &&
			GUILayout.Button("Generate"))
		{
			DoGenerate();
		}
	}

	private MapTileScript SpawnTile(Vector3 pos)
	{
		GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab);
		obj.transform.position = pos;
		obj.transform.parent = this.tiles.transform;
		MapTileScript script = obj.GetComponent<MapTileScript>();
		return script;
	}

	private void DoGenerate()
	{
		// Create a table as large as the terrain
		// Spawn tiles everywhere on the terrain and store them in the table
		// Once that's done, double parse the table to link every tile to the tiles it can see
		Transform terrainTransform = this.terrain.transform;
		Vector3 startPos = terrainTransform.position - new Vector3(5 * terrainTransform.localScale.x, 0, 5 * terrainTransform.localScale.z);

		for (float i = -5 * terrainTransform.localScale.x; i < 5 * terrainTransform.localScale.x; i++)
		{
			for (float j = -5 * terrainTransform.localScale.z; j < 5 * terrainTransform.localScale.z; j++)
			{
				SpawnTile(new Vector3(i, 0, j));
			}
		}
	}
}
