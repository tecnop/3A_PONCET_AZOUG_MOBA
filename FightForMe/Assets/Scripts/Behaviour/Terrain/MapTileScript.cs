using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapTileScript : MonoBehaviour
{
	//[SerializeField]
	private MapTile tile;

	void Start()
	{
		if (tile == null)
		{ // We shouldn't even be here to begin with...
			Destroy(this.gameObject);
		}
	}

	public void SetTile(MapTile tile)
	{
		this.tile = tile;
	}

	void OnDrawGizmosSelected()
	{
		foreach (MapTile other in tile.GetNeighbours())
		{
			Gizmos.DrawLine(tile.position, other.position);
		}
	}
}
