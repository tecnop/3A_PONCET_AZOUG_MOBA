using UnityEngine;
using System.Collections;

/*
 * CharacterManager.cs
 * 
 * Initializes all the character's scripts and allows them to communicate with each other
 * 
 */

public class CharacterManager : MonoBehaviour
{
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

	private Transform _transform;

	void Start()
	{
		_transform = this.transform;

		// Link all the serialized scripts to us and initialize them
		// THIS ORDER IS IMPORTANT (TO AN EXTENT)
		if (_camera != null && isLocal)
		{
			_camera.Initialize(this);
		}
		_inventory.Initialize(this);
		_combat.Initialize(this);
		_stats.Initialize(this);
		_event.Initialize(this);
		_vision.Initialize(this);
		if (_misc != null)
		{
			_misc.Initialize(this);
		}
		_animator.Initialize(this);
		_movement.Initialize(this);
		_input.Initialize(this);
		_physics.Initialize(this);
	}

	void Update()
	{
		if (GameData.gamePaused)
		{ // No updating
			return;
		}

		_vision.UpdateVision();

		_input.UpdateInput();
		_movement.ApplyMove(_input.GetDirectionalInput());
		_movement.SetAngle(_input.GetIdealOrientation());
		
		if (_camera)
		{
			_camera.UpdateCamera();
		}

		_combat.UpdateBuffs();

		_stats.ApplyRegen();
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
}
