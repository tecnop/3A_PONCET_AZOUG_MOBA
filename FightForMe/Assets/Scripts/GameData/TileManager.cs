﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TileManager
{
	private static List<List<MapTile>> tiles;

	private static float tileSpacing = 1.0f;

	private static Vector3 firstPos;

	private static MapTile globalTile;

	public static MapTile GetTileForPos(Vector3 pos)
	{
		Vector3 relPos = (pos - firstPos) / tileSpacing;

		//return GetTile(Mathf.FloorToInt(relPos.x), Mathf.FloorToInt(relPos.z));
		return GetTile(Mathf.RoundToInt(relPos.x), Mathf.RoundToInt(relPos.z));
	}

	public static MapTile GetTile(int x, int y)
	{
		if (tiles != null)
		{
			if (x >= 0 && x < tiles.Count &&
				y >= 0 && y < tiles[x].Count)
			{
				return tiles[x][y];
			}
		}
		else
		{ // Erm... We haven't even been initialized, just do something
			if (globalTile == null)
			{
				globalTile = new MapTile(Vector3.zero);
			}
			return globalTile;
		}
		return null;
	}

	public static void SetTiles(MapTile[][] newTiles)
	{
		tiles = new List<List<MapTile>>();
		if (newTiles != null)
		{
			foreach (MapTile[] row in newTiles)
			{
				tiles.Add(new List<MapTile>());
				foreach (MapTile tile in row)
				{
					tiles[tiles.Count - 1].Add(tile);
				}
			}

			if (tiles.Count > 0 && tiles[0].Count > 0)
			{
				firstPos = tiles[0][0].position;
			}
		}
	}

	public static void GenerateTiles(Vector3 startPos, int countX, int countZ, float spacing)
	{
		tiles = new List<List<MapTile>>();

		tileSpacing = spacing;

		for (int i = 0; i < countX; i++)
		{
			List<MapTile> curRow = new List<MapTile>();

			float x = startPos.x + tileSpacing * i;

			for (int j = 0; j < countZ; j++)
			{
				float z = startPos.z + tileSpacing * j;

				curRow.Add(new MapTile(new Vector3(x, 0, z)));
			}

			tiles.Add(curRow);
		}

		if (tiles.Count > 0 && tiles[0].Count > 0)
		{
			firstPos = tiles[0][0].position;
		}
	}

	public static void BuildNeighbours()
	{
		for (int _i = 0; _i < tiles.Count; _i++)
		{
			for (int _j = 0; _j < tiles[_i].Count; _j++)
			{
				tiles[_i][_j].ClearNeighbours();
			}
		}

		for (int _i = 0; _i < tiles.Count; _i++)
		{
			for (int _j = 0; _j < tiles[_i].Count; _j++)
			{
				MapTile curTile = tiles[_i][_j];
				int startingJ = _j + 1;
				for (int i = _i; i < tiles.Count; i++)
				{
					for (int j = startingJ; j < tiles[i].Count; j++)
					{
						MapTile otherTile = tiles[i][j];

						curTile.TryMakeNeighbourWith(otherTile);
					}
					startingJ = 0;
				}
			}
		}
	}
}
