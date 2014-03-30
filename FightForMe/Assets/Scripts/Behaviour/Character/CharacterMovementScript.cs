using UnityEngine;
using System.Collections;

/*
 * CharacterMovementScript.cs
 * 
 * Translates received input in character movement and rotation
 * 
 */

public class CharacterMovementScript : MonoBehaviour
{
	private CharacterManager _manager;

	private Transform _characterTransform;

	private CharacterController _controller;

	private Transform _myTransform;

	private CharacterStatsScript _stats;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_myTransform = this.transform;
		_characterTransform = _manager.GetCharacterTransform();
		_controller = this.GetComponent<CharacterController>();

		_stats = _manager.GetStatsScript();
	}

	public void ApplyMove(Vector3 dir)
	{ // TODO: When knockback is implemented, replace the controller with a rigidbody
		_controller.Move(dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f));

		Vector3 actualMove = _myTransform.position - _characterTransform.position;

		// Reset our position now that we found the correct value
		_myTransform.position = _characterTransform.position;

		_characterTransform.position += actualMove;

		_manager.GetCharacterAnimator().SetFloat("speed", actualMove.magnitude / Time.deltaTime);
	}

	public void LookAtPosition(Vector3 pos)
	{
		Vector3 diff = pos - _characterTransform.position;
		float yaw;

		if (diff.z == 0)
		{
			if (diff.x > 0)
			{
				yaw = 90;
			}
			else
			{
				yaw = -90;
			}
		}

		if (diff.z > 0)
		{
			yaw = Mathf.Atan(diff.x / diff.z) * 180 / Mathf.PI;
		}
		else
		{
			yaw = 180 + Mathf.Atan(diff.x / diff.z) * 180 / Mathf.PI;
		}

		_characterTransform.rotation = Quaternion.Euler(0, yaw, 0);
	}
}
