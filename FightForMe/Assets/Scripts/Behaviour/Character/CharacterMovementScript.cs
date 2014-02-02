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

	private CharacterEventScript _event;
	private CharacterStatsScript _stats;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_myTransform = this.transform;
		_characterTransform = _manager.GetCharacterTransform();
		_controller = this.GetComponent<CharacterController>();

		_event = _manager.GetEventScript();
		_stats = _manager.GetStatsScript();
	}

	public void ApplyMove(Vector3 dir)
	{
		_controller.Move(dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed()/100.0f));

		Vector3 actualMove = _myTransform.position - _characterTransform.position;

		// Reset our position now that we found the correct value
		_myTransform.position = _characterTransform.position;

		_characterTransform.position += actualMove;

		_manager.GetCharacterAnimator().SetFloat("speed", actualMove.magnitude / Time.deltaTime);
	}

	public void SetAngle(float yaw)
	{
		//float save = _characterTransform.eulerAngles.y;
		_characterTransform.rotation = Quaternion.Euler(0, yaw, 0);
	}
}
