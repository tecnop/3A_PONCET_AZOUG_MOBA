using UnityEngine;
using System.Collections;

enum AIType { defensive, aggressive, roaming };

public class NPCAIScript : CharacterInputScript {

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

	// AI-Helping variables
	private bool goalReached;
	private GameObject target;
	private Transform targetTransform;
	private Vector3 startPos;

	void Start()
	{
		myTransform = this.transform;
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
		startPos = _misc.GetSpawnPos();
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
		else if (startPos != null)
		{
			SetGoal(startPos);
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
		goalPosition = pos;
	}

	public override Vector3 GetDirectionalInput()
	{
		RunAI ();

		if (goalReached)
		{
			return Vector3.zero;
		}

		Vector3 move = goalPosition - myTransform.position;
		move.y = 0;

		if (move.magnitude < approachRange)
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
			return Mathf.Atan(diff.x/diff.z) * 180 / Mathf.PI;
		}
		else
		{
			return 180 + Mathf.Atan(diff.x/diff.z) * 180 / Mathf.PI;
		}
	}

}
