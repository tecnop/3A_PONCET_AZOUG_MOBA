using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class TileGenerator : EditorWindow
{
	private static TileGenerator _tileGen;

	// Options
	private GameObject tilePrefab;
	private Transform terrain;
	private Transform tiles;
	private TileBuilderScript tileBuilder;
	private float tileSpacing = 3.0f;

	private List<TileEntityScript> debugTiles;
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

		if (temp != null && temp.GetComponent<TileEntityScript>() != null)
		{
			this.tilePrefab = temp;
		}

		Transform temp2;

		temp2 = (Transform)EditorGUILayout.ObjectField("Ground entity", this.terrain, typeof(Transform), true);

		if (temp2 != null && PrefabUtility.GetPrefabType(temp2) != PrefabType.Prefab)
		{
			this.terrain = temp2;
		}

		temp2 = (Transform)EditorGUILayout.ObjectField("Tiles parent", this.tiles, typeof(Transform), true);

		if (temp2 != null && PrefabUtility.GetPrefabType(temp2) != PrefabType.Prefab)
		{
			this.tiles = temp2;
		}

		TileBuilderScript temp3;

		temp3 = (TileBuilderScript)EditorGUILayout.ObjectField("Tile builder", this.tileBuilder, typeof(TileBuilderScript), true);

		if (temp3 != null && PrefabUtility.GetPrefabType(temp3) != PrefabType.Prefab)
		{
			this.tileBuilder = temp3;
		}
		
		GUILayout.Space(10);

		if (this.tileCount > 0)
		{
			if (this.tiles != null)
			{
				if (this.debugTiles != null && this.debugTiles.Count > 0)
				{
					if (GUILayout.Button("Hide Editor tiles"))
					{
						HideEditorTiles();
					}
				}
				else
				{
					if (GUILayout.Button("Enable Editor tiles"))
					{
						DisplayEditorTiles();
					}
				}
			}
			if (GUILayout.Button("Build neighbours"))
			{
				TileManager.BuildNeighbours();
			}
			if (GUILayout.Button("Clear all " + this.tileCount + " tiles"))
			{
				DestroyTiles();
			}
		}
		else if (this.tilePrefab != null && this.terrain != null && this.tileBuilder != null)
		{
			this.tileSpacing = EditorGUILayout.FloatField("Spacing", this.tileSpacing);
			if (GUILayout.Button("Generate"))
			{
				DoGenerate();
			}
		}
	}

	private TileEntityScript SpawnEditorTile(MapTile tile)
	{
		GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab);
		obj.transform.position = tile.position;
		obj.transform.parent = this.tiles;
		TileEntityScript script = obj.GetComponent<TileEntityScript>();
		script.SetTile(tile);
		tile.SetEntity(script);
		return script;
	}

	private void DoGenerate()
	{
		float terrainSizeX = 10.0f * this.terrain.localScale.x;
		float terrainSizeZ = 10.0f * this.terrain.localScale.z;

		Vector3 startPos = new Vector3(-0.5f * terrainSizeX, 0, -0.5f * terrainSizeZ);

		int sizeX = Mathf.CeilToInt(terrainSizeX / tileSpacing);
		int sizeZ = Mathf.CeilToInt(terrainSizeZ / tileSpacing);

		this.tileCount = sizeX * sizeZ;

		TileManager.GenerateTiles(startPos, sizeX, sizeZ, tileSpacing);

		TileManager.ExportTilesToBuilder(this.tileBuilder);
	}

	private void DisplayEditorTiles()
	{
		this.debugTiles = new List<TileEntityScript>();

		int i = 0;
		do
		{
			int j = 0;
			MapTile tile = TileManager.GetTile(i, j);
			while (tile != null)
			{
				if (tile != null)
				{
					this.debugTiles.Add(SpawnEditorTile(tile));
				}

				tile = TileManager.GetTile(i, ++j);
			}

			i++;

		} while (TileManager.GetTile(i, 0) != null); // Awkward
	}

	private void HideEditorTiles()
	{
		int length = this.debugTiles.Count;
		int i;
		for (i = 0; i < length; i++)
		{
			DestroyImmediate(this.debugTiles[i].gameObject);
		}

		this.debugTiles = null;

		i = 0;
		do
		{
			int j = 0;
			MapTile tile = TileManager.GetTile(i, j);
			while (tile != null)
			{
				if (tile != null)
				{
					tile.SetEntity(null);
				}

				tile = TileManager.GetTile(i, ++j);
			}

			i++;

		} while (TileManager.GetTile(i, 0) != null); // Awkward
	}

	private void DestroyTiles()
	{
		if (this.debugTiles != null)
		{
			HideEditorTiles();
		}

		TileManager.ClearTiles();

		this.tileCount = 0;
	}
}
