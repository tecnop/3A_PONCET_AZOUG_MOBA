using UnityEngine;
using System.Collections;

/*
 * CharacterPhysicsScript.cs
 * 
 * Acts as an interface between the character and other colliders
 * 
 */

public class CharacterPhysicsScript : MonoBehaviour
{
	[SerializeField]
	private NetworkView _networkView;

	private CharacterManager _manager;

	private int pendingLayer = 0;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;

		if (pendingLayer != 0)
		{
			_SetLayer(pendingLayer);
		}
	}

	void OnTriggerEnter(Collider collider)
	{ // Redirect it
		_manager.GetEventScript().OnCollision(collider);
	}

	[RPC]
	private void _SetLayer(int layer)
	{
		if (_manager == null)
		{
			pendingLayer = layer;
			return;
		}

		_manager.gameObject.layer = layer;
		this.gameObject.layer = layer;
	}

	public void SetLayer(int layer)
	{
		if (GameData.isOnline)
		{
			_networkView.RPC("_SetLayer", RPCMode.All, layer);
		}
		else
		{
			_SetLayer(layer);
		}
	}

	public CharacterManager GetManager()
	{ // This is required for proper interactions with colliders
		return _manager;
	}
}
