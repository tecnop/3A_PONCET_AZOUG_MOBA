using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * CharacterVisionScript.cs
 * 
 * Defines which entities and positions the character can see
 * 
 */

public class CharacterVisionScript : MonoBehaviour
{
	private CharacterManager _manager;

	private MapTile curTile;
	private List<MapTile> tilesInSight;
	private List<VisibleEntity> entitiesInSight;
	private float lastUpdate;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;

		this.lastUpdate = 0.0f;

		this.curTile = null;
		this.tilesInSight = new List<MapTile>();
		this.entitiesInSight = new List<VisibleEntity>();

		//UpdateVision(true);
	}

	public bool CanSee(Vector3 pos)
	{
		MapTile tile = TileManager.GetTileForPos(pos);

		return (tile == _manager.GetCurrentTile() || this.tilesInSight.Contains(tile));
	}

	public bool CanSee(Transform transform)
	{
		return CanSee(transform.position);
	}

	public bool CanSee(CharacterManager character)
	{
		return CanSee(character.GetCharacterTransform());
	}

	public void AddSeenEntity(VisibleEntity entity)
	{
		if (entity != _manager && !this.entitiesInSight.Contains(entity))
		{
			this.entitiesInSight.Add(entity);
			_manager.GetEventScript().OnSpotEntity(entity);
		}
	}

	public void RemoveSeenEntity(VisibleEntity entity)
	{
		if (this.entitiesInSight.Contains(entity))
		{
			this.entitiesInSight.Remove(entity);
			_manager.GetEventScript().OnLoseSightOfEntity(entity);
		}
	}

	public void UpdateVision()
	{
		bool firstTime = (this.lastUpdate == 0.0f);

		if (!firstTime && Time.time - lastUpdate < 0.2f)
		{ // Vision updates at most 5 times per second
			return;
		}
		lastUpdate = Time.time;

		// Update our current tile
		bool doUpdate = false;
		try
		{
			doUpdate = _manager.UpdatePositionOnGrid();
		}
		catch
		{
			Debug.LogWarning(_manager.name + " moved outside of the vision grid");
		}

		if (!doUpdate)
		{ // We haven't moved
			return;
		}

		MapTile newTile = _manager.GetCurrentTile();
		List<MapTile> newTiles = newTile.GetNeighbours();

		if (this.curTile)
		{ // Remove us from the old tiles we can't see anymore
			if (!newTiles.Contains(this.curTile))
			{
				this.curTile.RemoveSubscriber(this);
			}
			foreach (MapTile neighbour in this.tilesInSight)
			{
				if (neighbour != newTile && !newTiles.Contains(neighbour))
				{
					neighbour.RemoveSubscriber(this);
				}
			}
		}

		// Add us to the new tiles we can now see
		List<VisibleEntity> newEntities = newTile.ObjectsInside();
		if (!this.tilesInSight.Contains(newTile))
		{
			newTile.AddSubscriber(this);
		}
		foreach (MapTile neighbour in newTiles)
		{
			newEntities.AddRange(neighbour.ObjectsInside());
			if (neighbour != this.curTile && !this.tilesInSight.Contains(neighbour))
			{
				neighbour.AddSubscriber(this);
			}
		}

		this.curTile = newTile;
		this.tilesInSight = newTiles;

		//if (!firstTime)
		{
			bool doCheck = (this.entitiesInSight.Count > 0);

			foreach (VisibleEntity ent in newEntities)
			{
				if (ent != _manager.gameObject)
				{
					bool found = false;

					if (doCheck)
					{ // Get rid of the ones we already knew about
						foreach (VisibleEntity ent2 in this.entitiesInSight)
						{
							if (ent == ent2)
							{
								found = true;
								break;
							}
						}
					}

					if (!found)
					{
						_manager.GetEventScript().OnSpotEntity(ent);
						//Debug.Log(_manager.name + " spotted " + ent.name);

						if (ent is CharacterManager)
						{ // Force him to see us so he doesn't have to run a full update
							//Debug.Log(ent.name + " spotted " + _manager.name + " back");
							((CharacterManager)ent).GetVisionScript().AddSeenEntity(_manager);
						}
					}
				}
			}
		}

		List<VisibleEntity> lostEntities = new List<VisibleEntity>(this.entitiesInSight);

		foreach (VisibleEntity ent in this.entitiesInSight)
		{
			if (newEntities.Contains(ent))
			{
				lostEntities.Remove(ent);
			}
		}

		foreach (VisibleEntity ent in lostEntities)
		{
			_manager.GetEventScript().OnLoseSightOfEntity(ent);

			/*if (ent is CharacterManager)
			{ // Force him to lose us so he doesn't have to run a full update
				((CharacterManager)ent).GetVisionScript().RemoveSeenEntity(_manager);
			}*/
		}

		this.entitiesInSight = newEntities;
	}

	public List<VisibleEntity> GetEntitiesInSight()
	{
		if (this.entitiesInSight != null)
		{
			return new List<VisibleEntity>(this.entitiesInSight);
		}
		else
		{
			return new List<VisibleEntity>();
		}
	}
}
