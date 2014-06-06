using UnityEngine;
using System.Collections;

/*
 * NetworkedEntityScript
 * 
 * Serves as an interface for entities that need to be sent to clients
 * 
 */

public class NetworkedEntityScript : MonoBehaviour
{
	[SerializeField]
	protected NetworkView _networkView;

	private bool initialized;

	void Start()
	{
		if (!GameData.isOnline)
		{ // Not waiting for anything
			initialized = true;
			//Initialize();
		}
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		if (!initialized && Network.connections.Length == (int)GameData.expectedConnections)
		{
			initialized = true;
			//Initialize();
			_networkView.RPC("RemoteInit", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	private void RemoteInit()
	{
		//Initialize();
	}

	/*virtual void Initialize()
	{
		Debug.LogWarning("Networked entity " + this.name + " is missing an initialization function");
	}*/
}
