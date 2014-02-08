using UnityEngine;
using System.Collections;

/*
 * CharacterVisionScript.cs
 * 
 * Defines which entities and positions the character can see
 * 
 */

public class CharacterVisionScript : MonoBehaviour
{
	private CharacterManager _manager;

	private CharacterEventScript _event;

	private Transform _transform;
	private Collider _collider;

	private GameObject[] entitiesInSight;
	private float lastUpdate;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;

		lastUpdate = Time.time;
		_event = _manager.GetEventScript();
		_collider = _manager.GetPhysicsScript().collider;
		_transform = this.transform;

		entitiesInSight = new GameObject[1];
		entitiesInSight[0] = _manager.gameObject;	// This should not change! It makes sure we don't spot ourselves
	}

	public bool IsPosVisible(Vector3 pos)
	{ // TODO: Use capsule casts instead?
		Vector3 diff = (_transform.position - pos).normalized;
		RaycastHit rayInfo;
		Physics.Raycast(pos, diff, out rayInfo);

		return (rayInfo.collider == _collider);
	}

	public void UpdateVision()
	{
		if (Time.time - lastUpdate < 0.2)
		{ // Vision updates at most 5 times per second
			return;
		}
		lastUpdate = Time.time;

		// TODO: Line of sight check

		// ==== DEBUG ====
		GameObject[] newEntities = GameObject.FindGameObjectsWithTag("Player");
		// ==== DEBUG ====

		// Get rid of the ones we already knew about
		foreach (GameObject obj in newEntities)
		{
			bool found = false;

			if (this.entitiesInSight.Length > 0)
			{
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
				this._event.OnSpotEntity(obj);
			}
		}

		// TODO: Handle entities we lost sight of?

		this.entitiesInSight = newEntities;
	}
}
