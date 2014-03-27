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

			if (_manager.GetStatsScript().GetHealth() <= 0)
			{ // We still execute the update code because of reasons
				newInput = Vector3.zero;
				newAngle = idealOrientation;
			}

			ReadGenericInput(); // This one handles events by itself... store it in a struct maybe?
			SetDirectionalInput(newInput);
			SetIdealOrientation(newAngle);

			if (GameData.isOnline)
			{
				_networkView.RPC("SetDirectionalInput", RPCMode.Others, newInput);
				_networkView.RPC("SetIdealOrientation", RPCMode.Others, newAngle);
			}
		}
	}

	protected void SetAttackState(bool state, int spellNum)
	{
		_SetAttackState(state, spellNum);

		if (GameData.isOnline)
		{
			_networkView.RPC("_SetAttackState", RPCMode.Others, state, spellNum);
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
	private void _SetAttackState(bool state, int spellNum)
	{
		if (!_manager)
		{ // ????
			return;
		}

		_manager.GetCharacterAnimator().SetBool("isAttacking", state);
		_manager.GetCharacterAnimator().SetInteger("spellNum", spellNum);
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
