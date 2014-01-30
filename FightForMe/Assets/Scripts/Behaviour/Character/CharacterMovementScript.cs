using UnityEngine;
using System.Collections;
/*
 * Update : Mr.P 30 01 2014
 * > Ajout d'une entrée sur l'Animator pour la gestion de la vitesse actuel ("speed" dans l'Animator) du perso dans ApplyMove().
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

	void OnTriggerEnter(Collider collider)
	{ // Redirect it
		_event.OnCollision(collider);
	}

	public void ApplyMove(Vector3 dir)
	{
		//_controller.SimpleMove(dir);
		_controller.Move(dir.normalized * Time.deltaTime * (_stats.GetMovementSpeed()/100.0f));

		Vector3 actualMove = _myTransform.position - _characterTransform.position;

		// Reset our position now that we found the correct value
		_myTransform.position = _characterTransform.position;

		_characterTransform.position += actualMove;

		if (_manager.GetCharacterAnimator() != null)
		{

			_manager.GetCharacterAnimator().SetFloat("speed", actualMove.magnitude);

			/*
			if (actualMove.magnitude > 0.001f && _manager.GetCharacterAnimator().GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Idle"))
			{
				Debug.Log("Now moving");
				_manager.GetCharacterAnimator().Play("Moving");
			}
			else if (_manager.GetCharacterAnimator().GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Moving"))
			{
				Debug.Log("Now idle");
				_manager.GetCharacterAnimator().Play("Idle");
			}
			*/
		}
	}

	public void SetAngle(float yaw)
	{
		//float save = _characterTransform.eulerAngles.y;
		_characterTransform.rotation = Quaternion.Euler(0, yaw, 0);
	}

	public CharacterManager GetManager()
	{ // This is required for proper interactions with colliders
		return _manager;
	}
}
