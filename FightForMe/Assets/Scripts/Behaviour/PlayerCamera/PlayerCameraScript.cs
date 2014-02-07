using UnityEngine;
using System.Collections;

public class PlayerCameraScript : MonoBehaviour
{
	private CharacterManager _manager;
	private PlayerInputScript _input;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private PlayerHUDScript _hudScript;

	private Transform _characterTransform;
	private Transform _myTransform;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_input = (PlayerInputScript)_manager.GetInputScript();

		_characterTransform = _manager.GetCharacterTransform();
		_myTransform = this.transform;

		_hudScript._Initialize(manager);
	}

	public Camera GetCamera()
	{
		return _camera;
	}

	public Vector3 GetCameraPos()
	{
		return _myTransform.position;
	}

	public void UpdateCamera()
	{
		if (_input.HasLockedCamera())
		{
			_myTransform.position = _characterTransform.position;
		}
		else
		{
			Vector3 mousePos = _input.GetMousePos();

			// Retrieving the position as a percentage of the screen's resolution
			float mousePosX = mousePos.x / Screen.width;
			float mousePosY = mousePos.y / Screen.height;

			float moveX = 0;
			float moveY = 0;

			if (mousePosX > 0.9f)
			{
				moveX = Mathf.Clamp(10 * (mousePosX - 0.9f), 0.0f, 2.0f);
			}
			else if (mousePosX < 0.1f)
			{
				moveX = -Mathf.Clamp(10 * (0.1f - mousePosX), 0.0f, 2.0f);
			}

			if (mousePosY > 0.9f)
			{
				moveY = Mathf.Clamp(10 * (mousePosY - 0.9f), 0.0f, 2.0f);
			}
			else if (mousePosY < 0.1f)
			{
				moveY = -Mathf.Clamp(10 * (0.1f - mousePosY), 0.0f, 2.0f);
			}

			Vector3 move = new Vector3(moveX, 0, moveY);

			_myTransform.position += move;
		}
	}
}
