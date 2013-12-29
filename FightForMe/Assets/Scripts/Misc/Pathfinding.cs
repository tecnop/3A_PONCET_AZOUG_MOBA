using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Pathfinding
{
	private static Dictionary<uint, WayPointScript> nodeTable = new Dictionary<uint, WayPointScript>();

	public static uint AddNode(WayPointScript script)
	{
		uint id = GetNumNodes();
		nodeTable.Add(id, script);
		return id;
	}

	public static uint GetNumNodes()
	{ // Convinience function
		return (uint)nodeTable.Count;
	}

	public static Vector3 GetNodePos(uint index)
	{
		if (index >= GetNumNodes())
		{
			return Vector3.zero;
		}

		return nodeTable[index].GetPos();
	}

	private static uint GetClosestWayPoint(Vector3 pos)
	{ // Returns the closest visible waypoint to the position NOTE: If none was found, returns nodeTable.Count
		uint tableLength = GetNumNodes();
		uint curIndex = tableLength;
		float curDist = -1;

		for (uint i = 0; i < tableLength; i++)
		{
			float dist = Vector3.Distance(pos, nodeTable[i].GetPos());

			if (dist < curDist || curDist == -1)
			{ // TODO: Add visibility check
				curIndex = i;
				curDist = dist;
			}
		}

		return curIndex;
	}

	private static uint[] AStar(uint startNode, uint endNode)
	{ // A* goes here
		uint[] res = new uint[2];
		res[0] = 2;
		res[1] = 0;
		return res;
	}

	public static uint[] GetPath(Vector3 start, Vector3 end)
	{ // Returns the list of waypoints to follow to get from "start" to "end"
		uint startNode = GetClosestWayPoint(start);
		uint endNode = GetClosestWayPoint(end);

		if (startNode >= GetNumNodes() || endNode >= GetNumNodes())
		{ // ERROR: Origin or destination point has no visible waypoint
			return null;
		}

		return AStar(startNode, endNode);
	}
}
