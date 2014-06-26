using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MapGrid : ScriptableObject
{
	[SerializeField]
	private List<MapRow> tiles;	// Matrix of tiles

	[SerializeField]
	private Vector3 firstPos;			// Bottom left corner of the grid

	[SerializeField]
	private float tileSpacing;			// Distance between each tile

	[SerializeField] // I didn't want to serialize it because I don't want it to be editable but it keeps being deleted so
	private int _totalTiles;

	public int totalTiles
	{
		get
		{
			return _totalTiles;
		}
	}

	public void SetUp(List<MapRow> tiles, float spacing, Vector3 firstPos)
	{
		this.name = "Map Grid";

		this.tiles = tiles;
		this.firstPos = firstPos;
		this.tileSpacing = spacing;

		this._totalTiles = 0;
		for (int i = 0; i < tiles.Count; i++)
		{
			this._totalTiles += tiles[i].Count;
		}
	}

	public MapTile GetTileForPos(Vector3 pos)
	{
		Vector3 relPos = (pos - firstPos) / tileSpacing;

		return GetTile(Mathf.RoundToInt(relPos.x), Mathf.RoundToInt(relPos.z));
	}

	public MapTile GetTile(int x, int y)
	{
		if (tiles != null)
		{
			if (x >= 0 && x < tiles.Count &&
				y >= 0 && y < tiles[x].Count)
			{
				return tiles[x][y];
			}
		}
		return null;
	}

	public int RowCount()
	{
		return this.tiles.Count;
	}

	public int RowSize(int index)
	{
		return this.tiles[index].Count;
	}
}
