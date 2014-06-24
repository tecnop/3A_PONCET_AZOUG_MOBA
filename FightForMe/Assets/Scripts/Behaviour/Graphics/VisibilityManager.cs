using UnityEngine;
using System;
using System.Collections;

public class VisibilityManager : MonoBehaviour
{ // I made this to replace VisibleEntity and then I remembered why I had settled for that solution in the first place
	/*[SerializeField]
	private Transform _transform;

	[SerializeField]
	private GraphicsLoader _graphics;

	private MapTile curTile;

	void Start()
	{
		try
		{
			UpdatePositionOnGrid();
		}
		catch
		{
			Debug.LogWarning(this.gameObject.name + " is not on a tile! Its coordinates are " + _transform.position);
		}
	}

	public bool UpdatePositionOnGrid()
	{ // Returns true if we moved to a new tile, false otherwise
		MapTile newTile = TileManager.GetTileForPos(_transform.position);

		if (newTile == null)
		{ // Use that to make projectiles disappear maybe?
			throw new Exception("No tile found for position " + _transform.position);
		}

		if (curTile != null)
		{
			if (newTile == curTile)
			{ // We haven't moved from the previous tile
				return false;
			}
			curTile.RemoveEntity(this);
		}
		newTile.AddEntity(this);

		this.curTile = newTile;

		return true;
	}

	public MapTile GetCurrentTile()
	{
		return curTile;
	}

	public void SetVisible(bool visible)
	{
		_graphics.gameObject.SetActive(visible);
	}*/
}
