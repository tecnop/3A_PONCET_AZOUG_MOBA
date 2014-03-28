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

	private Vector3 directionalInput;
	private float idealOrientation;
	private uint currentSpell;

	public abstract void Initialize(CharacterManager manager);

	protected abstract Vector3 UpdateDirectionalInput();
	protected abstract float UpdateIdealOrientation();
	protected abstract uint UpdateCurrentSpell();
	protected abstract void ReadGenericInput();

	public void UpdateInput()
	{
		if (_manager.IsLocal())
		{
			Vector3 newInput = UpdateDirectionalInput();
			float newAngle = UpdateIdealOrientation();
			uint newSpell = UpdateCurrentSpell();

			if (_manager.GetStatsScript().GetHealth() <= 0)
			{ // We still execute the update code because of reasons
				newInput = Vector3.zero;
				newAngle = idealOrientation;
				newSpell = 0;
			}

			ReadGenericInput(); // This one handles events by itself... store it in a struct maybe?

			SetDirectionalInput(newInput);
			SetIdealOrientation(newAngle);
			SetCurrentSpell((int)newSpell);

			if (GameData.isOnline)
			{ // There's no reason to send 3 RPCs here, we should send only one
				_networkView.RPC("SetDirectionalInput", RPCMode.Others, newInput);
				_networkView.RPC("SetIdealOrientation", RPCMode.Others, newAngle);
				_networkView.RPC("SetCurrentSpell", RPCMode.Others, (int)newSpell);
			}
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
	private void SetCurrentSpell(int spell)
	{
		this.currentSpell = (uint)spell;
		_manager.GetCharacterAnimator().SetBool("isAttacking", (spell != 0)); // Kinda temporary
	}

	public Vector3 GetDirectionalInput()
	{
		return this.directionalInput;
	}

	public float GetIdealOrientation()
	{
		return this.idealOrientation;
	}

	public uint GetCurrentSpell()
	{
		return this.currentSpell;
	}
}
