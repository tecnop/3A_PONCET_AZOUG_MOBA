using UnityEngine;
using System.Collections;

public class CharacterVisionScript : MonoBehaviour {

	[SerializeField]
	CharacterManager _manager;

	private CharacterEventScript _event;

	private GameObject[] entitiesInSight;
	private float lastUpdate;

	void Start()
	{
		lastUpdate = Time.time;
		_event = _manager.GetEventScript();
		entitiesInSight = new GameObject[1];
		entitiesInSight[0] = _manager.gameObject;	// This should not change! It makes sure we don't spot ourselves
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
