using UnityEngine;
using System.Collections;

public class DetectionSphereScript : MonoBehaviour
{
	private ArrayList entities;

	private float startTime;

	void Start()
	{
		startTime = Time.time;
		entities = new ArrayList();
		Debug.Log("Spawned");
	}

	public void storeData(Vector3 position, float radius, int layer, float damage = 0.0f, uint buffID = 0, uint damageFlags = 0)
	{
		this.gameObject.layer = layer;
		Debug.Log("Stored data");
	}

	void OnTriggerEnter(Collider col)
	{
		entities.Add(col);
		Debug.Log("Collision");
	}

	void LateUpdate()
	{
		if (Time.time < startTime + 0.2f)
		{ // No time left
			return;
		}

		Destroy(this.gameObject);
		Debug.Log("Killed");
	}
}
