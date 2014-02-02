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
	private CharacterManager _manager;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
	}

	void OnTriggerEnter(Collider collider)
	{ // Redirect it
		_manager.GetEventScript().OnCollision(collider);
	}

	public CharacterManager GetManager()
	{ // This is required for proper interactions with colliders
		return _manager;
	}
}
