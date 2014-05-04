using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class MapTile : ScriptableObject
{
	private Vector3 _position;			// Position of the tile on the map
	public Vector3 position
	{
		get
		{
			return _position;
		}
	}

	private Vector2 _size;				// Size of the tile on the floor
	public Vector2 size
	{
		get
		{
			return _size;
		}
	}

	private List<MapTile> _neighbours;	// Neighbours of this tile
	private List<GameObject> _objects;	// Objects currently in this tile

	private TileEntityScript tileEntity;// Entity meant to represent us in the editor

	public void Initialize(Vector3 position, Vector2 size)
	{
		this._position = position;
		this._size = size;
		this._neighbours = new List<MapTile>();
		this._objects = new List<GameObject>();
		this.tileEntity = null;

		//this.hideFlags = HideFlags.NotEditable;
	}

	public void SetEntity(TileEntityScript tileEntity)
	{ // This is now unnecessary, keeping it temporarily just in case
		this.tileEntity = tileEntity;
	}

	public bool CanSee(MapTile tile)
	{ // TODO: Use capsule casts instead?
		Vector3 diff = tile.position - this.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(this.position + new Vector3(0, 1, 0), diff.normalized, out hitInfo, diff.magnitude, (1 << LayerMask.NameToLayer("Terrain"))))
		{ // Something is blocking the line of sight
			if (TileManager.GetTileForPos(hitInfo.point) == tile)
			{ // The hit point is inside the tile, we're good
				return true;
			}
			return false;
		}
		return true;
	}

	public void TryMakeNeighbourWith(MapTile other)
	{
		if (this.CanSee(other) && other.CanSee(this))
		{
			this._neighbours.Add(other);
			other._neighbours.Add(this);
		}
	}

	public bool RemoveEntity(GameObject entity)
	{
		if (this._objects.Contains(entity))
		{
			this._objects.Remove(entity);
			return true;
		}
		return false;
	}

	public bool AddEntity(GameObject entity)
	{
		if (!this._objects.Contains(entity))
		{
			this._objects.Add(entity);
			return true;
		}
		return false;
	}

	public void ClearNeighbours()
	{
		this._neighbours.Clear();
	}

	public List<MapTile> GetNeighbours()
	{
		return new List<MapTile>(this._neighbours);
	}

	public List<GameObject> ObjectsInside()
	{
		return new List<GameObject>(this._objects);
	}
}
