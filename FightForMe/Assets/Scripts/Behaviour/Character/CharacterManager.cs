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

	void Start()
	{
		_transform = this.transform;
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
}
