using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class TileGenerator : EditorWindow
{
	private static TileGenerator _tileGen;

	// Options
	private GameObject tilePrefab;
	private Transform tiles;
	private TileBuilderScript tileBuilder;
	private float tileSpacing = 3.0f;

	private List<TileEntityScript> debugTiles;

	private static GUIStyle titleStyle;

	private TileGenerator()
	{

	}

	public static void Open()
	{
		if (_tileGen == null)
		{
			_tileGen = (TileGenerator)CreateInstance("TileGenerator");
		}

		titleStyle = new GUIStyle();
		titleStyle.alignment = TextAnchor.MiddleCenter;
		titleStyle.fontStyle = FontStyle.Bold;

		_tileGen.Show();
	}

	void OnGUI()
	{
		if (titleStyle == null)
		{ // Script has been reloaded (this causes an error because of recursive GUI drawing but whatever)
			Debug.LogWarning("TileGenerator has been reloaded while the window was active (you may ignore the following error)");
			this.Close();
			return;
		}

		GUILayout.Label("Generation", titleStyle);

		TileBuilderScript temp3 = (TileBuilderScript)EditorGUILayout.ObjectField("Tile builder", this.tileBuilder, typeof(TileBuilderScript), true);

		if (temp3 != null && PrefabUtility.GetPrefabType(temp3) != PrefabType.Prefab)
		{
			this.tileBuilder = temp3;

			if (TileManager.GridSize() == -1 && this.tileBuilder.HasGrid())
			{
				this.tileBuilder.LoadGrid();
			}
		}
		
		GUILayout.Space(10);

		if (TileManager.GridSize() > 0)
		{
			if (GUILayout.Button("Build neighbours"))
			{
				TileManager.BuildNeighbours();
			}
			if (GUILayout.Button("Clear all " + TileManager.GridSize() + " tiles"))
			{
				DestroyTiles();
			}

			GUILayout.Space(10);

			//EditorGUILayout.Separator();

			GUILayout.Space(10);

			GUILayout.Label("Debugging", titleStyle);

			GameObject temp = (GameObject)EditorGUILayout.ObjectField("Tile prefab", this.tilePrefab, typeof(GameObject), false);

			if (temp != null && temp.GetComponent<TileEntityScript>() != null)
			{
				this.tilePrefab = temp;
			}

			Transform temp2 = (Transform)EditorGUILayout.ObjectField("Tiles parent", this.tiles, typeof(Transform), true);

			if (temp2 != null && PrefabUtility.GetPrefabType(temp2) != PrefabType.Prefab)
			{
				this.tiles = temp2;
			}

			if (this.debugTiles != null && this.debugTiles.Count > 0)
			{
				if (GUILayout.Button("Hide Editor tiles"))
				{
					HideEditorTiles();
				}
			}
			else if (this.tilePrefab != null)
			{
				if (GUILayout.Button("Enable Editor tiles"))
				{
					DisplayEditorTiles();
				}
			}
		}
		else if (this.tileBuilder != null)
		{
			this.tileSpacing = EditorGUILayout.FloatField("Spacing", this.tileSpacing);
			if (GUILayout.Button("Generate"))
			{
				DoGenerate();
			}
		}

		// TODO: EditorGUI.ProgressBar
	}

	private TileEntityScript SpawnEditorTile(MapTile tile)
	{
		GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab);
		obj.transform.position = tile.position;
		obj.transform.parent = this.tiles;
		TileEntityScript script = obj.GetComponent<TileEntityScript>();
		script.SetTile(tile);
		//tile.SetEntity(script);
		return script;
	}

	private void DoGenerate()
	{
		tileBuilder.Generate(false);

		TileManager.ExportGridToBuilder(this.tileBuilder);
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

		/*i = 0;
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

		} while (TileManager.GetTile(i, 0) != null); // Awkward*/
	}

	private void DestroyTiles()
	{
		if (this.debugTiles != null)
		{
			HideEditorTiles();
		}

		TileManager.ClearTiles();

		TileManager.ExportGridToBuilder(this.tileBuilder);
	}
}
