using UnityEngine;
using System.Collections;

public class PlayerInputScript : CharacterInputScript
{
	[SerializeField]
	Camera _camera;

	[SerializeField]
	Transform _playerTransform;

	public override Vector3 GetDirectionalInput()
	{
		return new Vector3(Input.GetAxis("HorzMove"), 0, Input.GetAxis("VertMove"));
	}

	public override float GetIdealOrientation()
	{
		Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit rayInfo;
		Vector3 diff;

		if (Physics.Raycast(ray, out rayInfo, 100.0f, (1 << LayerMask.NameToLayer("Terrain"))))
		{
			diff = rayInfo.point - _playerTransform.position;
		}
		else
		{ // FIXME: Take the camera position into account (fire a raycast to the middle of the screen and get the diff from that or something)
			diff = new Vector3(Input.mousePosition.x - Screen.width / 2, 0, Input.mousePosition.y - Screen.height / 2);
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
}
