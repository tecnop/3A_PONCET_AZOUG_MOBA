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

		UpdateVision(true);
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

	public void UpdateVision(bool firstTime = false)
	{
		if (!firstTime && Time.time - lastUpdate < 0.2)
		{ // Vision updates at most 5 times per second
			return;
		}
		lastUpdate = Time.time;

		// Update our current tile
		MapTile newTile = TileManager.GetTileForPos(_transform.position);

		if (newTile == null)
		{ // Use that to make projectiles disappear maybe?
			Debug.LogError("ERROR: " + _manager.name + " is not on a tile!");
			return;
		}
		if (newTile == curTile)
		{ // We haven't moved from the previous tile
			return;
		}

		if (curTile != null)
		{
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

		if (!firstTime)
		{
			bool doCheck = (this.entitiesInSight != null && this.entitiesInSight.Count > 0);

			foreach (GameObject obj in newEntities)
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
				{ // TODO: Tell that entity to spot us too so they don't have to run a full update
					_manager.GetEventScript().OnSpotEntity(obj);
				}
			}
		}

		// TODO: Handle entities we lost sight of?

		this.entitiesInSight = newEntities;
	}
}
