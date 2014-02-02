using UnityEngine;
using System.Collections;

/*
 * CharacterInputScript.cs
 * 
 * Reads the character's input
 * 
 */

public abstract class CharacterInputScript : MonoBehaviour
{
	protected CharacterManager _manager;
	protected NetworkView _networkView;

	protected Vector3 directionalInput;
	protected float idealOrientation;

	public abstract void Initialize(CharacterManager manager);

	protected abstract Vector3 UpdateDirectionalInput();
	protected abstract float UpdateIdealOrientation();
	protected abstract void ReadGenericInput();

	public void UpdateInput()
	{
		if (_manager.IsLocal())
		{
			Vector3 newInput = UpdateDirectionalInput();
			float newAngle = UpdateIdealOrientation();

			ReadGenericInput(); // This one handles events by itself

			SetDirectionalInput(newInput);
			SetIdealOrientation(newAngle);

			if (Network.isServer || Network.isClient)
			{
				_networkView.RPC("SetDirectionalInput", RPCMode.Others, newInput);
				_networkView.RPC("SetIdealOrientation", RPCMode.Others, newAngle);
			}
		}
	}

	[RPC]
	protected void SetDirectionalInput(Vector3 vec)
	{
		this.directionalInput = vec;
	}

	[RPC]
	protected void SetIdealOrientation(float yaw)
	{
		this.idealOrientation = yaw;
	}

	public Vector3 GetDirectionalInput()
	{
		return this.directionalInput;
	}

	public float GetIdealOrientation()
	{
		return this.idealOrientation;
	}
}
