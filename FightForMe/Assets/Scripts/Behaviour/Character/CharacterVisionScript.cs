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

	private Transform _transform;

	private MapTile curTile;
	private List<MapTile> tilesInSight;
	private List<GameObject> entitiesInSight;
	private float lastUpdate;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = _manager.GetCharacterTransform();

		this.curTile = null;

		this.lastUpdate = 0.0f;

		//UpdateVision(true);
	}

	public bool CanSee(Vector3 pos)
	{
		MapTile tile = TileManager.GetTileForPos(pos);

		return (tile == this.curTile || this.tilesInSight.Contains(tile));
	}

	public bool CanSee(Transform transform)
	{
		return CanSee(transform.position);
	}

	public bool CanSee(CharacterManager character)
	{
		return CanSee(character.GetCharacterTransform());
	}

	public void AddSeenEntity(GameObject entity)
	{
		if (!this.entitiesInSight.Contains(entity))
		{
			this.entitiesInSight.Add(entity);
			_manager.GetEventScript().OnSpotEntity(entity);
		}
	}

	public void RemoveSeenEntity(GameObject entity)
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
		MapTile newTile = TileManager.GetTileForPos(_transform.position);

		if (newTile == null)
		{ // Use that to make projectiles disappear maybe?
			Debug.LogWarning(_manager.name + " is not on a tile! His coordinates are " + _transform.position);
			return;
		}

		if (curTile != null)
		{
			if (newTile == curTile)
			{ // We haven't moved from the previous tile
				return;
			}
			curTile.RemoveEntity(_manager.gameObject);
		}
		newTile.AddEntity(_manager.gameObject);

		this.curTile = newTile;
		this.tilesInSight = newTile.GetNeighbours();

		// TODO: Only check the new tiles? Would that be more costly?
		List<GameObject> newEntities = this.curTile.ObjectsInside();

		foreach (MapTile tile in this.tilesInSight)
		{
			newEntities.AddRange(tile.ObjectsInside());
		}

		//if (!firstTime)
		{
			bool doCheck = (this.entitiesInSight != null && this.entitiesInSight.Count > 0);

			foreach (GameObject obj in newEntities)
			{
				if (obj != _manager.gameObject)
				{
					bool found = false;

					if (doCheck)
					{ // Get rid of the ones we already knew about
						foreach (GameObject obj2 in this.entitiesInSight)
						{
							if (obj == obj2)
							{
								found = true;
								break;
							}
						}
					}

					if (!found)
					{
						_manager.GetEventScript().OnSpotEntity(obj);
						//Debug.Log(_manager.name + " spotted " + obj.name);
						CharacterManager hisManager = obj.GetComponent<CharacterManager>();
						if (hisManager != null)
						{ // Force him to see us so he doesn't have to run a full update
							//Debug.Log(hisManager.name + " spotted " + _manager.name + " back");
							hisManager.GetVisionScript().AddSeenEntity(_manager.gameObject);
						}
					}
				}
			}
		}

		if (this.entitiesInSight != null)
		{
			List<GameObject> lostEntities = new List<GameObject>(this.entitiesInSight);

			foreach (GameObject obj in this.entitiesInSight)
			{
				if (newEntities.Contains(obj))
				{
					lostEntities.Remove(obj);
				}
			}

			foreach (GameObject obj in lostEntities)
			{
				_manager.GetEventScript().OnLoseSightOfEntity(obj);
				CharacterManager hisManager = obj.GetComponent<CharacterManager>();
				if (hisManager != null)
				{ // Force him to lose us so he doesn't have to run a full update
					hisManager.GetVisionScript().RemoveSeenEntity(_manager.gameObject);
				}
			}
		}

		this.entitiesInSight = newEntities;
	}

	void OnDestroy()
	{ // Kind of a hack I guess
		if (curTile != null)
		{
			curTile.RemoveEntity(_manager.gameObject);

			/*foreach (GameObject obj in entitiesInSight)
			{ // FIXME
				CharacterManager hisManager = obj.GetComponent<CharacterManager>();
				if (hisManager != null)
				{ // Remove us from his targets to avoid pointer errors
					hisManager.GetVisionScript().RemoveSeenEntity(_manager.gameObject);
				}
			}*/
		}
	}

	public List<GameObject> GetEntitiesInSight()
	{
		if (this.entitiesInSight != null)
		{
			return new List<GameObject>(this.entitiesInSight);
		}
		else
		{
			return new List<GameObject>();
		}
	}
}
