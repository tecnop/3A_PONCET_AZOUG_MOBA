using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MapRow : ScriptableObject
{ // Nothing much, but necessary for this stupid serialization. I'm basically making it work like a list.
	[SerializeField]
	private List<MapTile> tiles;

	public int Count
	{
		get
		{
			if (tiles != null)
				return tiles.Count;
			else
				return -1;
		}
	}

	public MapTile this[int index]
	{
		get
		{
			return tiles[index];
		}
	}

	public void SetUp(int index, List<MapTile> tiles)
	{
		this.name = "Map row "+index;

		this.tiles = tiles;
	}
}
