using UnityEngine;
using System.Collections;

/*
 * CharacterManager.cs
 * 
 * Initializes all the character's scripts and allows them to communicate with each other
 * 
 */

public class CharacterManager : MonoBehaviour//NetworkedEntityScript
{
	[SerializeField]
	private Transform _transform;

	[SerializeField]
	private CharacterInputScript _input;

	[SerializeField]
	private CharacterMovementScript _movement;

	[SerializeField]
	private CharacterEventScript _event;

	[SerializeField]
	private CharacterStatsScript _stats;

	[SerializeField]
	private CharacterVisionScript _vision;

	[SerializeField]
	private CharacterInventoryScript _inventory;

	[SerializeField]
	private CharacterCombatScript _combat;

	[SerializeField]
	private CharacterMiscDataScript _misc;

	[SerializeField]
	private PlayerCameraScript _camera;

	[SerializeField]
	private CharacterAnimatorScript _animator;

	[SerializeField]
	private CharacterPhysicsScript _physics;

	[SerializeField]
	private GraphicsLoader _graphics;

	private bool isLocal = false;	// If true, the entity's input will be processed locally, otherwise it will be fetched from the network

	private float _savedAnimatorSpeed;

	private bool _initialized = false;

	void Start()
	{
		// Link all the serialized scripts to us and initialize them
		// THIS ORDER IS IMPORTANT (TO AN EXTENT)
		if (_camera != null && isLocal)
		{
			_graphics.gameObject.SetActive(true);
			_camera.Initialize(this);
		}
		_inventory.Initialize(this);
		_combat.Initialize(this);
		if (_misc != null)
		{
			_misc.Initialize(this);
		}
		_animator.Initialize(this);
		_stats.Initialize(this);
		_event.Initialize(this);
		_vision.Initialize(this);
		_movement.Initialize(this);
		_input.Initialize(this);
		_physics.Initialize(this);

		_initialized = true;
	}

	void Update()
	{
		if (GameData.gamePaused)
		{ // No updating
			if (!_animator.IsPaused())
			{
				_animator.Pause();
			}
			return;
		}
		else if (_animator.IsPaused())
		{
			_animator.Unpause();
		}

		_vision.UpdateVision();

		_input.UpdateInput();
		if (_stats.GetHealth() > 0.0f)
		{
			_movement.ApplyMove(_input.GetDirectionalInput());
			_movement.LookAtPosition(_input.GetLookPosition());
		}

		if (_camera && isLocal)
		{
			_camera.UpdateCamera();
		}

		_combat.UpdateBuffs();

		_stats.ApplyRegen();
	}

	/*void LateUpdate()
	{
		_movement.ConfirmMove();
	}*/

	void OnGUI()
	{
		if (!_initialized || !_graphics.gameObject.activeSelf)
		{
			return;
		}

		Vector3 screenPos = GameData.activePlayer.GetCameraScript().GetCamera().WorldToScreenPoint(_transform.position + new Vector3(0,4,0));

		float w = 100, h = 50;

		GUI.BeginGroup(SRect.Make(screenPos.x - w/2.0f, Screen.height - screenPos.y - h/2.0f, w, h));

		GUI.Label(SRect.Make(0, 0, w, 0.75f * h, "character_name"), this.name, FFMStyles.centeredText);

		float curHealth = _stats.GetHealth();
		float maxHealth = _stats.GetMaxHealth();

		// Background
		GUI.Box(SRect.Make(0, 0.75f * h, w, 0.25f * h, "character_bar"), GUIContent.none);

		if (curHealth > 1)
		{ // Bar
			Rect rect = SRect.Make(0.0f, 0.75f * h, (curHealth / maxHealth) * w, 0.25f * h);
			if (_stats.HasSpecialEffect(MiscEffect.INVULNERABLE))
			{ // Kinda temporary
				GUIStyle style = new GUIStyle();

				style.normal.background = new Texture2D(1, 1);
				style.normal.background.SetPixel(0, 0, Color.yellow);
				style.normal.background.Apply();

				GUI.Box(rect, GUIContent.none, style);
			}
			else
			{
				GUI.Box(rect, GUIContent.none);
			}
		}

		GUI.EndGroup();
	}

	public void MakeLocal() { this.isLocal = true; }
	public bool IsLocal() { return isLocal; }

	public CharacterInputScript GetInputScript() { return _input; }
	public CharacterEventScript GetEventScript() { return _event; }
	public CharacterCombatScript GetCombatScript() { return _combat; }
	public CharacterStatsScript GetStatsScript() { return _stats; }
	public CharacterVisionScript GetVisionScript() { return _vision; }
	public CharacterMovementScript GetMovementScript() { return _movement; }
	public CharacterInventoryScript GetInventoryScript() { return _inventory; }
	public CharacterMiscDataScript GetMiscDataScript() { return _misc; }
	public PlayerCameraScript GetCameraScript() { return _camera; }
	public CharacterAnimatorScript GetAnimatorScript() { return _animator; }
	public CharacterPhysicsScript GetPhysicsScript() { return _physics; }

	public Transform GetCharacterTransform() { return _transform; }
	public Animator GetCharacterAnimator() { return _animator.GetAnimator(); }
	public GraphicsLoader GetGraphicsLoader() { return _graphics; }
	public int GetLayer() { return this.gameObject.layer; }
}
