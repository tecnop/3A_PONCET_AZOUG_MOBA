using UnityEngine;
using System.Collections;

public class BotAIScript : CharacterInputScript
{
	private Transform _transform;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = _manager.GetCharacterTransform();
	}

	protected override Vector3 UpdateDirectionalInput()
	{
		return Vector3.zero;
	}

	protected override Vector3 UpdateLookPosition()
	{
		return _transform.position;
	}

	protected override uint UpdateCurrentSpell()
	{
		if (_manager.GetStatsScript().GetHealth() <= 0)
		{ // Can't attack
			return 0;
		}

		return 0;
	}

	protected override void ReadGenericInput()
	{

	}
}
