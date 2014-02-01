using UnityEngine;
using System.Collections;

public class DetectionSphereScript : MonoBehaviour
{
	private ArrayList entities;

	void Start()
	{
		entities = new ArrayList();
		Debug.Log("Spawned");
	}

	void OnTriggerEnter(Collider col)
	{
		entities.Add(col);
		Debug.Log("Collision");
	}

	public ArrayList GetResults()
	{ // Return our results and destroy us
		Debug.Log("Killed");
		Destroy(this.gameObject);
		return this.entities;
	}
}
