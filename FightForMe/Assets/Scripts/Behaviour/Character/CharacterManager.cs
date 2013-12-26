using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {

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
	private CameraMovementScript _camera;

	void Start ()
	{

	}

	void Update ()
	{
		_movement.ApplyMove(_input.GetDirectionalInput());

		_movement.SetAngle(_input.GetIdealOrientation());

		if (_camera)
		{
			_camera.UpdateCamera();
		}
	}

	public CharacterEventScript GetEventScript() { return _event; }
	public CharacterCombatScript GetCombatScript() { return _combat; }
	public CharacterStatsScript GetStatsScript() { return _stats; }
	public CharacterInventoryScript GetInventoryScript() { return _inventory; }
	public CameraMovementScript GetCameraScript() { return _camera; }
}
