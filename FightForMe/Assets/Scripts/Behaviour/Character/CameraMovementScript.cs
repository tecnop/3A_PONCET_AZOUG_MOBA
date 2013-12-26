using UnityEngine;
using System.Collections;

public class CameraMovementScript : MonoBehaviour {

	[SerializeField]
	Transform characterTransform;

	Transform myTransform;

	Vector3	bufferedMove;
	float bufferedYaw;
	
	void Start () {
		myTransform = this.transform;
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
		myTransform.position += bufferedMove;

		if (bufferedYaw != 0)
		{
			myTransform.RotateAround(characterTransform.position, Vector3.up, bufferedYaw);
		}

		bufferedMove = Vector3.zero;
		bufferedYaw = 0;
	}
}
