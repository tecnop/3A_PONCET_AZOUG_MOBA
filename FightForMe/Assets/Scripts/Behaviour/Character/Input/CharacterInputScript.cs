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
	private Vector3 lookPosition;
	private uint currentSpell;

	public abstract void Initialize(CharacterManager manager);

	protected abstract Vector3 UpdateDirectionalInput();
	protected abstract Vector3 UpdateLookPosition();
	protected abstract uint UpdateCurrentSpell();
	protected abstract void ReadGenericInput();

	public void UpdateInput()
	{
		if (_manager.IsLocal())
		{
			Vector3 newInput = UpdateDirectionalInput();
			Vector3 newLookPos = UpdateLookPosition();
			uint newSpell = UpdateCurrentSpell();

			ReadGenericInput();

			if (_manager.GetStatsScript().GetHealth() <= 0)
			{ // We used to still execute some stuff but I don't think we have to anymore? In fact do we even need to update anything else than generic input?
				return;
			}

			if (_manager.GetStatsScript().HasSpecialEffect(MiscEffect.LOSS_OF_CONTROL) ||
				_manager.GetMovementScript().IsMovementOverriden())
			{ // Don't let him attack at all
				newSpell = 0;
			}

			_UpdateInput(newInput, newLookPos, (int)newSpell);

			if (GameData.isOnline)
			{
				_networkView.RPC("_UpdateInput", RPCMode.Others, newInput, newLookPos, (int)newSpell);
			}
		}
	}

	[RPC]
	private void _UpdateInput(Vector3 dirInput, Vector3 lookPos, int spell)
	{ // Put in a single function for lighter bandwidth usage
		if (!_manager)
		{ // I'm still not sure why this happens
			return;
		}

		this.directionalInput = dirInput;
		this.lookPosition = lookPos;
		this.currentSpell = (uint)spell;
		_manager.GetCharacterAnimator().SetBool("isAttacking", (spell != 0)); // Kinda temporary
	}

	/*[RPC]
	private void SetDirectionalInput(Vector3 vec)
	{
		this.directionalInput = vec;
	}

	[RPC]
	private void SetLookPosition(Vector3 pos)
	{
		this.lookPosition = pos;
	}

	[RPC]
	private void SetCurrentSpell(int spell)
	{
		this.currentSpell = (uint)spell;
		_manager.GetCharacterAnimator().SetBool("isAttacking", (spell != 0)); // Kinda temporary
	}*/

	public Vector3 GetDirectionalInput()
	{
		return this.directionalInput;
	}

	public Vector3 GetLookPosition()
	{
		return this.lookPosition;
	}

	public uint GetCurrentSpell()
	{
		return this.currentSpell;
	}
}
