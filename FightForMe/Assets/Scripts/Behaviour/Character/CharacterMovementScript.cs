using UnityEngine;
using System.Collections;

public class CharacterMovementScript : MonoBehaviour {

	[SerializeField]
	private Transform characterTransform;

	[SerializeField]
	private CharacterController	controller;

	[SerializeField]
	CharacterManager _manager;

	private Transform myTransform;

	private CameraMovementScript _camera;
	private CharacterEventScript _event;

	void Start()
	{
		myTransform = this.transform;
		_camera = _manager.GetCameraScript();
		_event = _manager.GetEventScript();
	}

	void OnTriggerEnter(Collider collider)
	{ // Redirect it
		if (_event)
		{
			_event.OnCollision(collider);
		}
	}

	public void ApplyMove(Vector3 dir)
	{
		controller.SimpleMove(dir);

		Vector3 actualMove = myTransform.position - characterTransform.position;

		// Reset our position now that we found the correct value
		myTransform.position = characterTransform.position;

		characterTransform.position += actualMove;

		if (_camera)
		{
			_camera.SaveCameraMove(-actualMove);
		}
	}

	public void SetAngle(float yaw)
	{
		float save = characterTransform.eulerAngles.y;
		characterTransform.rotation = Quaternion.Euler(0, yaw, 0);

		if (_camera)
		{
			_camera.SaveCameraRotation(save-yaw);
		}
	}
}
