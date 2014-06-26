using UnityEngine;
using System.Collections;

public class PlayerInputScript : CharacterInputScript
{
	private PlayerCameraScript _cameraScript;

	private Transform _playerTransform;
	private Camera _camera;

	private BotDecisionMaker _ai;

	private bool hasLockedCamera;

	public override void Initialize(CharacterManager manager)
	{
		//base.Initialize(manager);
		_manager = manager;

		if (GameData.gameType == GameType.Local && _manager != GameData.activePlayer)
		{ // This one's a bot
			_ai = new BotDecisionMaker(_manager);
		}
		else if (_manager.IsLocal())
		{ // Only link the camera to us if we're going to be using it
			_cameraScript = _manager.GetCameraScript();
			_camera = _cameraScript.GetCamera();
		}

		_playerTransform = _manager.GetCharacterTransform();
		hasLockedCamera = true;
	}

	protected override Vector3 UpdateDirectionalInput()
	{
		if (_ai != null)
		{
			return _ai.GetDirectionalInput();
		}
		else
		{
			return new Vector3(Input.GetAxis("HorzMove"), 0, Input.GetAxis("VertMove"));
		}
	}

	protected override Vector3 UpdateLookPosition()
	{
		if (_ai != null)
		{
			return _ai.GetLookPosition();
		}
		else
		{
			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayInfo;
			Vector3 diff;

			if (Physics.Raycast(ray, out rayInfo, 100.0f, (1 << LayerMask.NameToLayer("Terrain"))))
			{
				//diff = rayInfo.point - _playerTransform.position;
				return rayInfo.point;
			}
			else
			{ // How do we feel about doing that all the time instead of using a RayCast? Is it as accurate?
				Vector3 cameraDiff = _camera.WorldToScreenPoint(_cameraScript.GetCameraPos()) - _camera.WorldToScreenPoint(_playerTransform.position);

				diff = new Vector3(Input.mousePosition.x - Screen.width / 2 + cameraDiff.x, 0, Input.mousePosition.y - Screen.height / 2 + cameraDiff.y);

				return _playerTransform.position + diff;
			}
		}
	}

	protected override uint UpdateCurrentSpell()
	{
		if (_manager.GetStatsScript().GetHealth() <= 0)
		{ // Can't attack
			return 0;
		}

		if (_ai != null)
		{
			return _ai.GetCurrentSpell();
		}
		else
		{
			if (HUDRenderer.GetState() != HUDState.Default)
			{ // Not the prettiest way to do it but hey
				return 0;
			}

			for (SpellSlot i = SpellSlot.SLOT_0; i < SpellSlot.NUM_SLOTS; i++)
			{
				string axisName = "Ability" + ((int)(i + 1));

				if (Input.GetAxis(axisName) > 0)
				{
					uint spell = ((PlayerMiscDataScript)_manager.GetMiscDataScript()).GetSpellForSlot(i);
					if (_manager.GetCombatScript().CanUseSpell(spell))
					{
						return spell;
					}
				}
			}

			return 0;
		}
	}

	protected override void ReadGenericInput()
	{
		if (_ai != null)
		{
			_ai.RunAI();
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{ // Don't forget to turn that into an axis
				this.hasLockedCamera = !this.hasLockedCamera;
			}
		}
	}

	public Vector3 GetMousePos()
	{
		if (!_manager.IsLocal())
		{ // This won't be called anyway, but just making sure
			return Vector3.zero;
		}

		return Input.mousePosition;
	}

	public bool MouseIsInRect(Rect rect)
	{
		Vector2 actualPos = Utils.MousePosToScreenPos(GetMousePos());

		if (actualPos.x > rect.xMin && actualPos.x < rect.xMax &&
			actualPos.y > rect.yMin && actualPos.y < rect.yMax)
		{
			return true;
		}

		return false;
	}

	public bool HasLockedCamera()
	{
		return this.hasLockedCamera;
	}

	// Event handling (mostly for bots, but could be used later for visual effects)
	#region

	public void NotifyEntityNoticed(VisibleEntity entity)
	{
		if (_ai != null)
		{
			_ai.NoticeEntity(entity);
		}
	}

	public void NotifyEntityLost(VisibleEntity entity)
	{
		if (_ai != null)
		{
			_ai.ForgetEntity(entity);
		}
	}

	#endregion
}
