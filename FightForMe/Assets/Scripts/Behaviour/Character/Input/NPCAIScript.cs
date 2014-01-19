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
	private Transform targetTransform;
	private Vector3 startPos;
	private ArrayList currentPath;
	private Vector3 finalPathDest;

	void Start()
	{
		_characterTransform = _manager.GetCharacterTransform();
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
		_vision = _manager.GetVisionScript();
		this.startPos = _misc.GetSpawnPos();
		this.currentPath = new ArrayList();
	}

	public bool IsSearchingEnemy()
	{
		return !target && behaviour != AIType.defensive;
	}

	private void RunAI()
	{
		if (target)
		{
			SetGoal(targetTransform.position);
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

	public override Vector3 GetDirectionalInput()
	{
		RunAI();

		if (this.goalReached)
		{
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

	public override float GetIdealOrientation()
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

	public override void ReadGenericInput()
	{ // Here: more AI (attacking, etc.)

	}

	public void SetBehaviour(AIType behaviour)
	{
		this.behaviour = behaviour;
	}
}
