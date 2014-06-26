using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MapTile : ScriptableObject
{
	[SerializeField]
	private Vector3 _position;			// Position of the tile on the map
	public Vector3 position
	{
		get
		{
			return _position;
		}
	}

	[SerializeField]
	private Vector2 _size;				// Size of the tile on the floor
	public Vector2 size
	{
		get
		{
			return _size;
		}
	}

	[SerializeField]
	private List<MapTile> neighbours;	// Neighbours of this tile

	[SerializeField]
	private List<int> neighbourIndexes;	// Indexes of the neighbours of this tile

	[SerializeField] // Mostly for debugging I guess but why not
	private List<VisibleEntity> objects;	// Objects currently in this tile

	[SerializeField] // Also for debugging, but I think I need to leave it like this for now
	private List<CharacterVisionScript> subscribers;	// Vision scripts who subscribed to this tile and should be notified of its changes

	[SerializeField]
	private int index; // Starts at 1

	//private TileEntityScript tileEntity;// Entity meant to represent us in the editor

	public void SetUp(int index, Vector3 position, Vector2 size)
	{
		this.name = "Map tile " + index;

		this.index = index;
		this._position = position;
		this._size = size;
		this.neighbours = new List<MapTile>();
		this.objects = new List<VisibleEntity>();
		this.subscribers = new List<CharacterVisionScript>();
		this.neighbourIndexes = new List<int>();
		//this.tileEntity = null;
	}

	/*public void SetEntity(TileEntityScript tileEntity)
	{ // This is now unnecessary, keeping it temporarily just in case
		this.tileEntity = tileEntity;
	}*/

	public bool CanSee(MapTile tile)
	{ // I tried using capsule casts / sphere casts but it's not working out well... I left it running for an hour and it still wasn't finished, and I'm out of time for this
		Vector3 diff = tile.position - this.position;
		RaycastHit hitInfo;
		//if (Physics.SphereCast(this.position + Vector3.up, 0.5f, diff.normalized, out hitInfo, diff.magnitude, (1 << LayerMask.NameToLayer("Terrain"))))
		if (Physics.Raycast(this.position + new Vector3(0.0f, 1.0f, 0.0f), diff.normalized, out hitInfo, diff.magnitude, (1 << LayerMask.NameToLayer("Terrain"))))
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
		if (this.CanSee(other) || other.CanSee(this))
		{ // Used to be an "and" thing, but I figured this would solve quite a few problems
			//this.neighbours.Add(other);
			//other.neighbours.Add(this);
			this.neighbourIndexes.Add(other.index);
			other.neighbourIndexes.Add(this.index);
		}
	}

	public void BakeNeighbours()
	{
		foreach (int index in this.neighbourIndexes)
		{
			MapTile neighbour = TileManager.GetTileForIndex(index);
			if (neighbour)
			{
				this.neighbours.Add(neighbour);
			}
		}
	}

	public bool RemoveEntity(VisibleEntity entity)
	{
		if (this.objects.Contains(entity))
		{
			this.objects.Remove(entity);

			foreach (CharacterVisionScript sub in this.subscribers)
			{
				sub.RemoveSeenEntity(entity);
			}

			return true;
		}
		return false;
	}

	public bool AddEntity(VisibleEntity entity)
	{
		if (!this.objects.Contains(entity))
		{
			this.objects.Add(entity);

			foreach (CharacterVisionScript sub in this.subscribers)
			{
				sub.AddSeenEntity(entity);
			}

			return true;
		}
		return false;
	}

	public bool RemoveSubscriber(CharacterVisionScript sub)
	{
		if (this.subscribers.Contains(sub))
		{
			this.subscribers.Remove(sub);
			return true;
		}
		return false;
	}

	public bool AddSubscriber(CharacterVisionScript sub)
	{
		if (!this.subscribers.Contains(sub))
		{
			this.subscribers.Add(sub);
			return true;
		}
		return false;
	}

	public void ClearNeighbours()
	{
		this.neighbours.Clear();
	}

	public List<MapTile> GetNeighbours()
	{
		return new List<MapTile>(this.neighbours);
	}

	public List<VisibleEntity> ObjectsInside()
	{
		return new List<VisibleEntity>(this.objects);
	}

	public List<CharacterVisionScript> GetSubscribers()
	{
		return new List<CharacterVisionScript>(this.subscribers);
	}
}
