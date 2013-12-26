using UnityEngine;
using System.Collections;

public class NPCAIScript : CharacterInputScript {

	[SerializeField]
	private Vector3 goalPosition;

	[SerializeField]
	[Range(0.0f, 32.0f)]
	private float approachRange;

	private bool goalReached;

	private Transform myTransform;

	void Start()
	{
		myTransform = this.transform;
	}

	private void RunAI()
	{
		//goalPosition = myTransform.position;
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
