using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileEntityScript : MonoBehaviour
{ // Simple entity meant to represent vision tiles in the editor
	private MapTile tile;

	// This doesn't actually reflect transform data
	private Vector3 pos;
	private Vector3 size;

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
		this.size = new Vector3(tile.size.x, 5.0f, tile.size.y);
		this.pos = tile.position + (this.size.y / 2.0f) * Vector3.up;
	}

	void OnDrawGizmosSelected()
	{ // FIXME: With the new system, the neighbours aren't baked yet while in editor
		Gizmos.DrawCube(this.pos, this.size);
		foreach (MapTile other in tile.GetNeighbours())
		{
			Gizmos.DrawLine(tile.position, other.position);
		}
	}
}
