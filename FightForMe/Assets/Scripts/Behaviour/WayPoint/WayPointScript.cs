using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayPointScript : MonoBehaviour {
	
	private static Dictionary<uint, WayPointScript> nodeTable = new Dictionary<uint, WayPointScript>();

	[SerializeField]
	private WayPointScript[] _neighbours;

	private uint _id;
	private Vector3 _pos;
	private uint _flags;

	private static uint GetNumNodes()
	{ // Convinience function
		return (uint)nodeTable.Count;
	}

	void Start()
	{
		this._id = GetNumNodes();
		this._pos = this.transform.position;
		this._flags = 0;
		nodeTable.Add(this._id, this);
	}

	public static uint GetClosestWayPoint(Vector3 pos)
	{ // Returns the closest visible waypoint to the position NOTE: If none was found, returns nodeTable.Count
		uint tableLength = GetNumNodes();
		uint curIndex = tableLength;
		float curDist = -1;

		for (uint i=0; i<tableLength; i++)
		{
			float dist = Vector3.Distance(pos, nodeTable[i]._pos);

			if (dist < curDist || curDist == -1)
			{ // TODO: Add visibility check
				curIndex = i;
				curDist = dist;
			}
		}

		return curIndex;
	}

	private static uint[] Pathfinding(uint startNode, uint endNode)
	{ // A* goes here
		return null;
	}

	public static uint[] GetPath(Vector3 start, Vector3 end)
	{ // Returns the list of waypoints to follow to get from "start" to "end"
		uint startNode = GetClosestWayPoint(start);
		uint endNode = GetClosestWayPoint(end);

		if (startNode >= GetNumNodes() || endNode >= GetNumNodes())
		{ // ERROR: Origin or destination point has no visible waypoint
			return null;
		}

		return Pathfinding(startNode, endNode);
	}

	public override string ToString ()
	{
		return "Node "+this._id+" has "+this._neighbours.Length+" neighbour(s)";
	}
}
