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

	[SerializeField]
	private Transform _transform; // Transform of our own entity (CharacterPhysics)

	[SerializeField]
	private Rigidbody _rigidBody;

	private CharacterStatsScript _stats;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_characterTransform = _manager.GetCharacterTransform();
		_stats = _manager.GetStatsScript();
	}

	public void ApplyMove(Vector3 dir)
	{ // TODO: When knockback is implemented, replace the controller with a rigidbody
		Vector3 actualMove = Vector3.zero;
		if (false)
		{ // New system (I don't like it, also it doesn't work)
			RaycastHit rayHit;
			float speed = Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f);
			if (!_rigidBody.SweepTest(dir.normalized, out rayHit, speed))
			{
				actualMove = dir.normalized * speed;
			}
		}
		else
		{
			//_controller.Move(dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f));
			//actualMove = _transform.position - _characterTransform.position;
			// Reset our position now that we found the correct value
			//_transform.position = _characterTransform.position;

			// This will do for now but walls don't work anymore
			_rigidBody.MovePosition(_rigidBody.position + dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f));
			actualMove = _rigidBody.position - _transform.position;
			_rigidBody.position = _transform.position;

			//_rigidBody.position = _rigidBody.position + dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f);
			//_transform.position += dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f);

			//actualMove = _rigidBody.position - _transform.position;
			// Reset our position now that we found the correct value
			//_rigidBody.position = _transform.position;

			//_rigidBody.AddRelativeForce(dir.normalized * (_stats.GetMovementSpeed() / 100.0f), ForceMode.Acceleration);
			//_transform.Translate(dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f));
		}
		
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
