using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AIType { defensive, aggressive, roaming };

public class NPCAIScript : CharacterInputScript
{
	[SerializeField] // Serialized for debugging
	private Vector3 goalPosition;

	[SerializeField] // Serialized for debugging
	[Range(0.0f, 32.0f)]
	private float approachRange;

	[SerializeField] // Serialized for debugging
	private AIType behaviour;

	private Transform _transform;

	private MonsterMiscDataScript _misc;

	// AI-Helping variables
	private bool goalReached;
	private GameObject target;
	private CharacterManager targetManager;
	private Transform targetTransform;
	private Vector3 startPos;
	//private List<uint> currentPath;
	private Queue<Vector3> currentPath;
	private Vector3 finalGoalPos;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = _manager.GetCharacterTransform();
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
		this.startPos = _misc.GetSpawnPos();
		this.currentPath = new Queue<Vector3>();
		_networkView = this.GetComponent<NetworkView>();
		this.goalPosition = _transform.position;
	}

	public bool IsSearchingEnemy()
	{
		return !target && behaviour != AIType.defensive;
	}

	public bool HasAnEnemy()
	{
		return target != null;
	}

	private void RunAI()
	{
		if (target)
		{
			if (targetManager && targetManager.GetStatsScript().GetHealth() <= 0)
			{ // He died
				target = null;
				SetGoal(this.startPos);
			}
			else
			{
				SetGoal(targetTransform.position);
			}
		}
		else
		{
			if (this.behaviour == AIType.roaming)
			{ // TODO: Get to a random node
			}
			else
			{
				SetGoal(this.startPos);
			}
		}
	}

	public void SetTarget(GameObject target)
	{
		this.target = target;
		this.targetTransform = target.transform;
		this.targetManager = target.GetComponent<CharacterManager>();
	}

	public void SetGoal(Vector3 pos)
	{
		if (Vector3.Distance(pos, this.finalGoalPos) < 1.0f)// && this.currentPath.Count > 1)
		{ // He hasn't moved much and we're already on our way (note: this is making straight movement a bit laggy right now)
			return;
		}

		goalReached = false;

		this.finalGoalPos = pos;
		NavMeshPath path = new NavMeshPath();

		if (NavMesh.CalculatePath(_transform.position, pos, (1 << NavMesh.GetNavMeshLayerFromName("Default")), path))
		{
			this.currentPath = new Queue<Vector3>(path.corners);
			this.goalPosition = this.currentPath.Dequeue();
		}
	}

	protected override Vector3 UpdateDirectionalInput()
	{
		RunAI();

		if (this.goalReached)
		{ // This is never true, is it?
			return Vector3.zero;
		}

		Vector3 move = this.goalPosition - _transform.position;
		move.y = 0;

		if (move.magnitude < approachRange && this.HasAnEnemy() && this.currentPath.Count == 0)
		{ // If our current goal is our actual target, stay at some distance
			goalReached = true;
			return Vector3.zero;
		}

		if (move.magnitude < 0.25f)
		{
			if (this.currentPath.Count > 0)
			{
				this.goalPosition = this.currentPath.Dequeue();
			}
			else
			{
				this.goalPosition = _transform.position;
				this.goalReached = true;
				return Vector3.zero;
			}
		}

		return move.normalized;
	}

	private static Vector3 DiffNoY(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x - b.x, 0, a.z - b.z);
	}	

	protected override Vector3 UpdateLookPosition()
	{
		if (DiffNoY(_transform.position, goalPosition).magnitude < 0.1f)
		{ // Don't panick alright, just look forward
			return _transform.position + 5.0f * _transform.forward;
		}
		else
		{
			return goalPosition;
		}
	}

	protected override uint UpdateCurrentSpell()
	{
		if (_manager.GetStatsScript().GetHealth() <= 0)
		{ // Can't attack
			return 0;
		}

		if (target && goalReached && Vector3.Distance(_transform.position, targetTransform.position) <= approachRange * 1.1f)
		{ // Simple attack rule
			return 1;
		}

		return 0;
	}

	protected override void ReadGenericInput()
	{ // Should we do anything here? How about using this to run the AI instead?

	}

	public void SetBehaviour(AIType behaviour)
	{
		this.behaviour = behaviour;
	}
}
