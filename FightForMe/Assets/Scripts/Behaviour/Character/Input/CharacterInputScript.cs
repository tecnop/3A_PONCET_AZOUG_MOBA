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

			ReadGenericInput(); // This one handles events by itself... store it in a struct maybe?

			if (_manager.GetStatsScript().GetHealth() > 0)
			{
				SetDirectionalInput(newInput);
				SetIdealOrientation(newAngle);

				if (GameData.isOnline)
				{
					_networkView.RPC("SetDirectionalInput", RPCMode.Others, newInput);
					_networkView.RPC("SetIdealOrientation", RPCMode.Others, newAngle);
				}
			}
		}
	}

	protected void SetAttackState(bool state)
	{
		_SetAttackState(state);

		if (GameData.isOnline)
		{
			_networkView.RPC("_SetAttackState", RPCMode.Others, state);
		}
	}

	[RPC]
	private void SetDirectionalInput(Vector3 vec)
	{
		this.directionalInput = vec;
	}

	[RPC]
	private void SetIdealOrientation(float yaw)
	{
		this.idealOrientation = yaw;
	}

	[RPC]
	private void _SetAttackState(bool state)
	{
		_manager.GetCharacterAnimator().SetBool("isAttacking", state);
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
