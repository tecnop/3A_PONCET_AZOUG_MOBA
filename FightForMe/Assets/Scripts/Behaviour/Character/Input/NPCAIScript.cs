using UnityEngine;
using System.Collections;

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

	private Transform _characterTransform;

	private MonsterMiscDataScript _misc;
	private CharacterVisionScript _vision;

	// AI-Helping variables
	private bool goalReached;
	private GameObject target;
	private CharacterManager targetManager;
	private Transform targetTransform;
	private Vector3 startPos;
	private ArrayList currentPath;
	private Vector3 finalPathDest;

	public override void Initialize(CharacterManager manager)
	{
		//base.Initialize(manager);
		_manager = manager;
		_characterTransform = _manager.GetCharacterTransform();
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
		_vision = _manager.GetVisionScript();
		this.startPos = _misc.GetSpawnPos();
		this.currentPath = new ArrayList();
		_networkView = this.GetComponent<NetworkView>();
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
		goalReached = false;

		if (_vision.IsPosVisible(pos))
		{
			goalPosition = pos;
			this.currentPath.Clear();
		}
		else
		{
			if (this.currentPath.Count > 0 && Vector3.Distance(finalPathDest, pos) < 1.0f)
			{ // Let's avoid unnecessary calls for now... we only recalculate the path if the new goal is somewhat far away from the previous one
				return;
			}

			this.currentPath = Pathfinding.GetPath(_characterTransform.position, pos);
			this.finalPathDest = pos;

			if (this.currentPath.Count > 1)
			{
				int i = 1;
				int lastId = 0;
				while (i < this.currentPath.Count)
				{
					if (_vision.IsPosVisible(Pathfinding.GetNodePos((uint)this.currentPath[i])))
					{
						lastId = i;
					}
					i++;
				}

				if (lastId != 0)
				{
					//Debug.Log("Taking a shortcut; removing " + lastId + " nodes from my path");
					this.currentPath.RemoveRange(0, lastId);
				}
			}

			if (this.currentPath.Count > 0)
			{
				goalPosition = Pathfinding.GetNodePos((uint)this.currentPath[0]);
			}
		}
	}

	public void OnWayPointCollision(uint wpIndex)
	{
		if (this.currentPath.Count > 0)
		{
			int i = 0;
			while (i < this.currentPath.Count)
			{
				if ((uint)this.currentPath[i] == wpIndex)
				{
					this.currentPath.RemoveRange(0, i + 1);
					if (this.currentPath.Count > 0)
					{
						goalPosition = Pathfinding.GetNodePos((uint)this.currentPath[0]);
					}
				}
				i++;
			}
		}
	}

	protected override Vector3 UpdateDirectionalInput()
	{
		RunAI();

		if (this.goalReached)
		{ // This is never true, is it?
			return Vector3.zero;
		}

		Vector3 move = this.goalPosition - _characterTransform.position;
		move.y = 0;

		if (move.magnitude < approachRange && this.currentPath.Count == 0)
		{ // If our current goal is our actual target, stay at some distance
			goalReached = true;
			return Vector3.zero;
		}

		return move.normalized;
	}

	protected override float UpdateIdealOrientation()
	{
		Vector3 diff = goalPosition - _characterTransform.position;

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

	protected override uint UpdateCurrentSpell()
	{
		if (_manager.GetStatsScript().GetHealth() <= 0)
		{ // Can't attack
			return 0;
		}

		if (target && goalReached && Vector3.Distance(_characterTransform.position, targetTransform.position) <= approachRange * 1.1f)
		{ // Simple attack rule
			return 1;
		}

		return 0;
	}

	protected override void ReadGenericInput()
	{ // Should we do anything here?

	}

	public void SetBehaviour(AIType behaviour)
	{
		this.behaviour = behaviour;
	}
}
