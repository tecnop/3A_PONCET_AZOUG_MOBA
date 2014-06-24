using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibleEntity : MonoBehaviour
{
	[SerializeField]
	protected Transform _transform;

	[SerializeField]
	protected GraphicsLoader _graphics;

	private MapTile curTile;

	private float lastVisionUpdate;
	private bool removedFromGrid;

	void Start()
	{
		removedFromGrid = false;
		try
		{
			UpdatePositionOnGrid();
		}
		catch
		{
			Debug.LogWarning(this.gameObject.name + " did not spawn on a tile! Its coordinates are " + _transform.position);
		}
	}

	void OnDestroy()
	{ // Kind of a hack I guess
		if (removedFromGrid) return;

		this.RemoveFromGrid();
	}

	public void RemoveFromGrid()
	{
		if (curTile != null)
		{
			curTile.RemoveEntity(this);
			curTile = null;
		}

		SetVisible(false);

		removedFromGrid = true;
	}

	public bool UpdatePositionOnGrid()
	{ // Returns true if we moved to a new tile, false otherwise
		if (removedFromGrid) return false;

		MapTile newTile = TileManager.GetTileForPos(_transform.position);

		lastVisionUpdate = Time.time;

		if (newTile == null)
		{ // Use that to make projectiles disappear maybe?
			throw new System.Exception("No tile found for position " + _transform.position);
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

	public float TimeSinceLastUpdate()
	{
		return Time.time - lastVisionUpdate;
	}

	public void SetVisible(bool visible)
	{
		if (removedFromGrid) return;
		_graphics.gameObject.SetActive(visible);
	}
}
