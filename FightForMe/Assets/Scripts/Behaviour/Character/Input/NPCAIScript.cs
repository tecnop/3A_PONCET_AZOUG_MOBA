using UnityEngine;
using System.Collections;

enum AIType { defensive, aggressive, roaming };

public class NPCAIScript : CharacterInputScript
{

	[SerializeField]
	CharacterManager _manager;

	[SerializeField]
	private Vector3 goalPosition;

	[SerializeField]
	[Range(0.0f, 32.0f)]
	private float approachRange;

	[SerializeField]
	private AIType behaviour;

	private Transform myTransform;

	private MonsterMiscDataScript _misc;
	private CharacterVisionScript _vision;

	// AI-Helping variables
	private bool goalReached;
	private GameObject target;
	private Transform targetTransform;
	private Vector3 startPos;
	private ArrayList currentPath;

	void Start()
	{
		myTransform = this.transform;
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
		_vision = _manager.GetVisionScript();
		startPos = _misc.GetSpawnPos();
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
				SetGoal(startPos);
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
			//if (Vector3.Distance(goalPosition, pos) < 1.0f)
			if (this.currentPath.Count > 0)
			{ // Let's avoid unnecessary calls for now...
				return;
			}

			this.currentPath = new ArrayList(Pathfinding.GetPath(myTransform.position, pos));

			goalPosition = Pathfinding.GetNodePos((uint)this.currentPath[0]);
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

		if (goalReached)
		{
			return Vector3.zero;
		}

		Vector3 move = goalPosition - myTransform.position;
		move.y = 0;

		if (move.magnitude < approachRange && this.currentPath.Count == 0)
		{
			goalReached = true;
			return Vector3.zero;
		}

		return move.normalized;
	}

	public override float GetIdealOrientation()
	{
		Vector3 diff = goalPosition - myTransform.position;

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

}
