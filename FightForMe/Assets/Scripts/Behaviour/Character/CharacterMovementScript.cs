using UnityEngine;
using System.Collections;

public class CharacterMovementScript : MonoBehaviour
{
	[SerializeField]
	private CharacterManager _manager;

	//[SerializeField] // Don't need to serialize this anymore, we can get it from the manager
	private Transform _characterTransform;

	//[SerializeField]
	private CharacterController _controller;

	private Transform _myTransform;

	private CharacterEventScript _event;

	void Start()
	{
		_myTransform = this.transform;
		_characterTransform = _manager.GetCharacterTransform();
		_controller = this.GetComponent<CharacterController>();

		_event = _manager.GetEventScript();
	}

	void OnTriggerEnter(Collider collider)
	{ // Redirect it
		_event.OnCollision(collider);
	}

	public void ApplyMove(Vector3 dir)
	{
		_controller.SimpleMove(dir);

		Vector3 actualMove = _myTransform.position - _characterTransform.position;

		// Reset our position now that we found the correct value
		_myTransform.position = _characterTransform.position;

		_characterTransform.position += actualMove;
	}

	public void SetAngle(float yaw)
	{
		//float save = _characterTransform.eulerAngles.y;
		_characterTransform.rotation = Quaternion.Euler(0, yaw, 0);
	}
}
