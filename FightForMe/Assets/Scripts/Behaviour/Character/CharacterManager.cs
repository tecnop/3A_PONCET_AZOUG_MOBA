using UnityEngine;
using System.Collections;

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

	private Transform _transform;
	private Animator _animator;

	void Start()
	{
		_transform = this.transform;
		_animator = this.GetComponent<Animator>();

		// Link all the serialized scripts to us and initialize them
		// THIS ORDER IS IMPORTANT (TO AN EXTENT)
		if (_camera != null)
		{
			_camera.Initialize(this);
		}
		_inventory.Initialize(this);
		_combat.Initialize(this);
		_stats.Initialize(this);
		_event.Initialize(this);
		_vision.Initialize(this);
		if (_misc)
		{
			_misc.Initialize(this);
		}
		_movement.Initialize(this);
		_input.Initialize(this);
	}

	void Update()
	{
		_vision.UpdateVision();

		_movement.ApplyMove(_input.GetDirectionalInput());

		_movement.SetAngle(_input.GetIdealOrientation());

		_input.ReadGenericInput();

		if (_camera)
		{
			_camera.UpdateCamera();
		}
	}

	public CharacterInputScript GetInputScript() { return _input; }
	public CharacterEventScript GetEventScript() { return _event; }
	public CharacterCombatScript GetCombatScript() { return _combat; }
	public CharacterStatsScript GetStatsScript() { return _stats; }
	public CharacterVisionScript GetVisionScript() { return _vision; }
	public CharacterMovementScript GetMovementScript() { return _movement; }
	public CharacterInventoryScript GetInventoryScript() { return _inventory; }
	public CharacterMiscDataScript GetMiscDataScript() { return _misc; }
	public PlayerCameraScript GetCameraScript() { return _camera; }

	public Transform GetCharacterTransform() { return _transform; }
	public Animator GetCharacterAnimator() { return _animator; }
}
