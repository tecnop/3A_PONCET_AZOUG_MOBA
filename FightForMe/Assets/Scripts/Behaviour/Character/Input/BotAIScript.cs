using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotAIScript : CharacterInputScript
{
	private Transform _transform;

	private Queue<Vector3> currentPath;
	private Vector3 goalPosition;
	//private Vector3 finalGoalPos;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = _manager.GetCharacterTransform();
	}

	private void SetGoal(Vector3 pos)
	{
		//this.finalGoalPos = pos;
		NavMeshPath path = new NavMeshPath();

		if (NavMesh.CalculatePath(_transform.position, pos, (1 << NavMesh.GetNavMeshLayerFromName("Default")), path))
		{
			this.currentPath = new Queue<Vector3>(path.corners);
			this.goalPosition = this.currentPath.Dequeue();
		}
	}

	protected override Vector3 UpdateDirectionalInput()
	{
		Vector3 move = this.goalPosition - _transform.position;
		move.y = 0;

		if (move.magnitude < 0.25f)
		{ // Reached the current objective
			if (this.currentPath.Count > 0)
			{ // Switch to the next one
				this.goalPosition = this.currentPath.Dequeue();
			}
			else
			{
				this.goalPosition = _transform.position;
				return Vector3.zero;
			}
		}

		return move.normalized;
	}

	protected override Vector3 UpdateLookPosition()
	{
		return _transform.position;
	}

	protected override uint UpdateCurrentSpell()
	{
		if (_manager.GetStatsScript().GetHealth() <= 0)
		{ // Can't attack
			return 0;
		}

		return 0;
	}

	protected override void ReadGenericInput()
	{ // Extra code executed after the rest of the input

	}
}
