using UnityEngine;
using System.Collections;

public class CameraMovementScript : MonoBehaviour
{
	[SerializeField]
	private Transform _characterTransform;

	private Transform _myTransform;

	private Vector3 bufferedMove;
	private float bufferedYaw;

	void Start()
	{
		_myTransform = this.transform;
		bufferedMove = Vector3.zero;
		bufferedYaw = 0;
	}

	public void SaveCameraMove(Vector3 move)
	{
		bufferedMove += move;
	}

	public void SaveCameraRotation(float yawChange)
	{
		bufferedYaw += yawChange;
	}

	public void UpdateCamera()
	{
		_myTransform.position += bufferedMove;

		if (bufferedYaw != 0)
		{
			_myTransform.RotateAround(_characterTransform.position, Vector3.up, bufferedYaw);
		}

		bufferedMove = Vector3.zero;
		bufferedYaw = 0;
	}
}
