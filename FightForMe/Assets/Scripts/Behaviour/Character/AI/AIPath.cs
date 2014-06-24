using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPath
{
	private Queue<Vector3> points;

	public AIPath(NavMeshPath navMeshPath)
	{
		if (navMeshPath != null)
		{
			points = new Queue<Vector3>(navMeshPath.corners);
		}
		else
		{
			points = new Queue<Vector3>();
		}
	}

	public static AIPath CreatePath(Vector3 origin, Vector3 dest)
	{
		NavMeshPath path = new NavMeshPath();

		if (NavMesh.CalculatePath(origin, dest, (1 << NavMesh.GetNavMeshLayerFromName("Default")), path))
		{
			return new AIPath(path);
		}

		return new AIPath(null);
	}

	public bool HasNextGoal()
	{
		return points.Count > 0;
	}

	public Vector3 DequeueNextGoal()
	{
		return points.Dequeue();
	}
}
