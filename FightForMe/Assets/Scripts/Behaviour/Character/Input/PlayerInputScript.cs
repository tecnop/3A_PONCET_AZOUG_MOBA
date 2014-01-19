using UnityEngine;
using System.Collections;

public class PlayerInputScript : CharacterInputScript
{
	[SerializeField]
	private bool isActive;		// Is this the active player for the current build settings?

	private PlayerCameraScript _cameraScript;

	private Transform _playerTransform;
	private Camera _camera;

	private bool hasLockedCamera = false;

	void Start()
	{
		if (!isActive)
		{ // Delete our camera
			Destroy(_manager.GetCameraScript().gameObject);
		}
		else
		{
			_cameraScript = _manager.GetCameraScript();
			_camera = _cameraScript.GetCamera();
		}

		_playerTransform = _manager.GetCharacterTransform();
	}

	public override Vector3 GetDirectionalInput()
	{
		if (isActive)
		{
			return new Vector3(Input.GetAxis("HorzMove"), 0, Input.GetAxis("VertMove"));
		}
		else
		{ // Get it from the network
			return Vector3.zero;
		}
	}

	public override float GetIdealOrientation()
	{
		if (!isActive)
		{ // Get it from the network
			return 0.0f;
		}

		Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit rayInfo;
		Vector3 diff;

		if (Physics.Raycast(ray, out rayInfo, 100.0f, (1 << LayerMask.NameToLayer("Terrain"))))
		{
			diff = rayInfo.point - _playerTransform.position;
		}
		else
		{
			Vector3 cameraDiff = _camera.WorldToScreenPoint(_cameraScript.GetCameraPos()) - _camera.WorldToScreenPoint(_playerTransform.position);

			diff = new Vector3(Input.mousePosition.x - Screen.width / 2 + cameraDiff.x, 0, Input.mousePosition.y - Screen.height / 2 + cameraDiff.y);
		}

		if (diff.z == 0)
		{
			if (diff.x > 0)
			{
				return 90;
			}
			else
			{
				return -90;
			}
		}

		if (diff.z > 0)
		{
			return Mathf.Atan(diff.x / diff.z) * 180 / Mathf.PI;
		}
		else
		{
			return 180 + Mathf.Atan(diff.x / diff.z) * 180 / Mathf.PI;
		}
	}

	public override void ReadGenericInput()
	{
		if (!isActive)
		{ // Nothing to see here
			return;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.hasLockedCamera = !this.hasLockedCamera;
		}
	}

	public Vector3 GetMousePos()
	{
		if (!isActive)
		{ // Won't be needed
			return Vector3.zero;
		}

		return Input.mousePosition;
	}

	public bool HasLockedCamera()
	{
		return this.hasLockedCamera;
	}
}
