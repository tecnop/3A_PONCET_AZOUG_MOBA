using UnityEngine;
using System.Collections;

public class NetworkedEntityScript : MonoBehaviour
{
	[SerializeField]
	protected NetworkView _networkView;

	void Start()
	{ // Add us to the server-side list of networked entities

	}

	public void Synchronize()
	{

	}
}
