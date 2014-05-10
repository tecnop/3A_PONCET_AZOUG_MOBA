using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TileManager
{ // Utility class to allow easier access to the vision grid
	private static MapGrid grid;

	private static float _generationProgress;

	public static float generationProgress
	{
		get
		{
			return _generationProgress;
		}
	}

	public static MapTile GetTileForPos(Vector3 pos)
	{
		if (grid != null)
		{
			return grid.GetTileForPos(pos);
		}
		return null;
	}

	public static MapTile GetTileForIndex(int index)
	{
		if (grid != null)
		{
			int l = grid.RowCount();
			int x = (index-1) / l;
			int y = (index-1) % l;
			return grid.GetTile(x, y);
		}
		return null;
	}

	public static MapTile GetTile(int x, int y)
	{
		if (grid != null)
		{
			return grid.GetTile(x, y);
		}
		return null;
	}

	public static void SetGrid(MapGrid newGrid)
	{
		TileManager.grid = newGrid;
	}

	public static void GenerateTiles(Vector3 startPos, int countX, int countZ, float spacing)
	{
		ClearTiles();

		List<MapRow> tiles = new List<MapRow>();

		for (int i = 0; i < countX; i++)
		{
			List<MapTile> curRow = new List<MapTile>();

			float x = startPos.x + spacing * (i + 0.5f);

			for (int j = 0; j < countZ; j++)
			{
				float z = startPos.z + spacing * (j + 0.5f);

				MapTile tile = ScriptableObject.CreateInstance<MapTile>();
				tile.SetUp(i * countX + j + 1, new Vector3(x, 0, z), new Vector2(spacing, spacing));

				curRow.Add(tile);
			}

			MapRow actualRow = ScriptableObject.CreateInstance<MapRow>();
			actualRow.SetUp(i + 1, curRow);

			tiles.Add(actualRow);
		}

		grid = ScriptableObject.CreateInstance<MapGrid>();
		grid.SetUp(tiles, spacing, startPos);
	}

	public static void ClearTiles()
	{
		if (grid != null)
		{
			ScriptableObject.DestroyImmediate(grid);
			grid = null;
		}
	}

	public static void BuildNeighbours()
	{
		for (int _i = 0; _i < grid.RowCount(); _i++)
		{
			for (int _j = 0; _j < grid.RowSize(_i); _j++)
			{
				grid.GetTile(_i, _j).ClearNeighbours();
			}
		}

		_generationProgress = 0.0f;

		for (int _i = 0; _i < grid.RowCount(); _i++)
		{
			for (int _j = 0; _j < grid.RowSize(_i); _j++)
			{
				MapTile curTile = grid.GetTile(_i, _j);
				int startingJ = _j + 1;
				for (int i = _i; i < grid.RowCount(); i++)
				{
					for (int j = startingJ; j < grid.RowSize(i); j++)
					{
						MapTile otherTile = grid.GetTile(i, j);

						curTile.TryMakeNeighbourWith(otherTile);
					}
					startingJ = 0;
				}
			}

			_generationProgress += 1.0f / grid.RowCount();
		}
	}

	public static void BakeNeighbours()
	{
		for (int i = 0; i < grid.RowCount(); i++)
		{
			for (int j = 0; j < grid.RowSize(i); j++)
			{
				grid.GetTile(i, j).BakeNeighbours();
			}
		}
	}

	public static void ExportGridToBuilder(TileBuilderScript builder)
	{
		builder.SetGrid(grid);
		UnityEditor.EditorUtility.SetDirty(builder);
	}

	public static int GridSize()
	{
		if (grid != null)
		{
			return grid.totalTiles;
		}
		return -1;
	}
}
