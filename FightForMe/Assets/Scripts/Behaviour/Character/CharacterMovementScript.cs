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
	private CharacterController _controller;

	[SerializeField]
	private Rigidbody _rigidBody;

	private CharacterStatsScript _stats;

	private Vector3 overrideDir;
	private float overrideSpeed;
	private float overrideDuration;
	private float overrideTimeLeft;
	private bool overrideFade;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_characterTransform = _manager.GetCharacterTransform();
		_stats = _manager.GetStatsScript();
	}

	public void ApplyMove(Vector3 dir)
	{ // TODO: CLEAN THIS UP
		Vector3 actualMove = Vector3.zero;
		Vector3 actualDir = dir;
		float actualSpeed = _stats.GetMovementSpeed() / 100.0f;

		if (overrideTimeLeft > 0)
		{ // This system is temporarily being used by knockbacks because rigidbodies are crap, otherwise it should be used for dashes, taunts, etc
			actualSpeed = overrideSpeed;
			if (overrideFade)
			{
				actualSpeed *= (overrideTimeLeft / overrideDuration);
			}
			actualDir = overrideDir;
			overrideTimeLeft -= Time.deltaTime;
		}
		else if (_manager.GetStatsScript().HasSpecialEffect(MiscEffect.LOSS_OF_CONTROL))
		{
			actualDir = Vector3.zero;
			actualSpeed = 0.0f;
		}

		#region wip
		/*RaycastHit rayHit;
		float speed = Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f);
		if (!_rigidBody.SweepTest(dir.normalized, out rayHit, speed))
		{
			actualMove = dir.normalized * speed;
		}*/

		// This will do for now but walls don't work anymore
		//_rigidBody.MovePosition(_rigidBody.position + dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f));
		//actualMove = _rigidBody.position - _transform.position;
		//_rigidBody.position = _transform.position;

		//_rigidBody.position += dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f);
		//_rigidBody.AddForce(dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f));
		//_transform.position += dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f);

		//actualMove = _rigidBody.position - _transform.position;
		// Reset our position now that we found the correct value
		//_rigidBody.position = _transform.position;

		//_rigidBody.AddRelativeForce(dir.normalized * (_stats.GetMovementSpeed() / 100.0f), ForceMode.Acceleration);
		//_transform.Translate(dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed() / 100.0f));
		#endregion

		// Back to the old system for now...
		_controller.Move(actualDir.normalized * actualSpeed * Time.deltaTime);
		actualMove = _transform.position - _characterTransform.position;
		// Reset our position now that we found the correct value
		_transform.position = _characterTransform.position;

		_characterTransform.position += actualMove;

		_manager.GetCharacterAnimator().SetFloat("speed", actualMove.magnitude / Time.deltaTime);
	}

	public void ConfirmMove()
	{ // Should be called in late update
		//if (_manager.GetCameraScript())
		//Debug.Log(_transform.position + " " + _transform.position + " " + _rigidBody.position);

		/*Vector3 actualMove = _transform.position - _transform.position;
		_transform.position = _transform.position;

		_transform.position += actualMove;

		//Debug.Log(actualMove);

		_manager.GetCharacterAnimator().SetFloat("speed", actualMove.magnitude / Time.deltaTime);*/
	}

	public void LookAtPosition(Vector3 pos)
	{
		if (_manager.GetStatsScript().HasSpecialEffect(MiscEffect.LOSS_OF_CONTROL) ||
			IsMovementOverriden())
		{
			return;
		}

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

	public void SetMovementOverride(Vector3 dir, float speed, float duration, bool fade)
	{ // TODO: If already overriden, merge the effects? Maybe? Actually that sounds stupid
		this.overrideDir = dir;
		this.overrideSpeed = speed;
		this.overrideDuration = duration;
		this.overrideTimeLeft = duration;
		this.overrideFade = fade;
	}

	public bool IsMovementOverriden()
	{
		return (this.overrideTimeLeft > 0);
	}
}
