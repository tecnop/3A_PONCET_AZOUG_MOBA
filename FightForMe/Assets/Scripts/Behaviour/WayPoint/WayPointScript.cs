using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayPointScript : MonoBehaviour
{

	[SerializeField]
	private WayPointScript[] _neighbours;

	private uint _id;
	private Vector3 _pos;
	private uint _flags;

	void Start()
	{
		this._pos = this.transform.position;
		this._flags = 0;
		this._id = Pathfinding.AddNode(this);
	}

	public uint GetID()
	{
		return this._id;
	}

	public Vector3 GetPos()
	{
		return this._pos;
	}

	public override string ToString()
	{
		return "Node " + this._id + " has " + this._neighbours.Length + " neighbour(s)";
	}
}
