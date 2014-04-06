using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileGenerator : EditorWindow
{
	private static TileGenerator _tileGen;

	// Options
	private GameObject tilePrefab;
	private Transform terrain;
	private Transform tiles;
	private float tileSpacing = 3.0f;

	private MapTileScript[][] debugMatrix;
	private int tileCount;

	private TileGenerator()
	{

	}

	public static void Open()
	{
		if (_tileGen == null)
		{
			_tileGen = (TileGenerator)CreateInstance("TileGenerator");
		}

		_tileGen.Show();
	}

	void OnGUI()
	{
		GameObject temp;

		temp = (GameObject)EditorGUILayout.ObjectField("Tile prefab", this.tilePrefab, typeof(GameObject), false);

		if (temp != null && temp.GetComponent<MapTileScript>() != null)
		{
			this.tilePrefab = temp;
		}

		Transform temp2;

		temp2 = (Transform)EditorGUILayout.ObjectField("Terrain entity", this.terrain, typeof(Transform), true);

		if (temp2 != null && PrefabUtility.GetPrefabType(temp2) != PrefabType.Prefab)
		{
			this.terrain = temp2;
		}

		temp2 = (Transform)EditorGUILayout.ObjectField("Tiles parent", this.tiles, typeof(Transform), true);

		if (temp2 != null && PrefabUtility.GetPrefabType(temp2) != PrefabType.Prefab)
		{
			this.tiles = temp2;
		}

		GUILayout.Space(10);

		if (this.tiles != null)
		{
			int count;
			if (this.tiles.childCount > 0)
			{
				count = this.tiles.childCount;
			}
			else
			{
				count = this.tileCount;
			}
			if (count > 0)
			{ // Restore buttons here
				if (GUILayout.Button("Build neighbours"))
				{
					TileManager.BuildNeighbours();
				}
				if (GUILayout.Button("Clear all " + count + " tiles"))
				{
					DestroyTiles();
				}
			}
			else if (this.tilePrefab != null && this.terrain != null)
			{
				this.tileSpacing = EditorGUILayout.FloatField("Spacing", this.tileSpacing);
				if (GUILayout.Button("Generate"))
				{
					DoGenerate();
				}
			}
		}
	}

	private MapTileScript SpawnDebugTile(MapTile tile)
	{
		GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab);
		obj.transform.position = tile.position;
		obj.transform.parent = this.tiles;
		MapTileScript script = obj.GetComponent<MapTileScript>();
		script.SetTile(tile);
		return script;
	}

	private void DoGenerate()
	{
		float terrainSizeX = 10.0f * this.terrain.localScale.x;
		float terrainSizeZ = 10.0f * this.terrain.localScale.z;

		Vector3 startPos = new Vector3(-0.5f * terrainSizeX, 0, -0.5f * terrainSizeZ);

		int sizeX = Mathf.FloorToInt(terrainSizeX / tileSpacing);
		int sizeZ = Mathf.FloorToInt(terrainSizeZ / tileSpacing);

		TileManager.GenerateTiles(startPos, sizeX, sizeZ, tileSpacing);

		this.tileCount = 0;
		
		this.debugMatrix = new MapTileScript[sizeX][];

		for (int i = 0; i < sizeX; i++)
		{
			this.debugMatrix[i] = new MapTileScript[sizeZ];

			for (int j = 0; j < sizeZ; j++)
			{
				this.debugMatrix[i][j] = SpawnDebugTile(TileManager.GetTile(i, j));

				this.tileCount++;
			}
		}
	}

	private void DestroyTiles()
	{
		int length = this.tiles.childCount;
		for (int i = 0; i < length; i++)
		{
			DestroyImmediate(this.tiles.GetChild(0).gameObject);
		}

		this.debugMatrix = null;

		this.tileCount = 0;
	}
}
