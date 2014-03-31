using UnityEngine;
using System.Collections;

public class MapTileScript : MonoBehaviour
{
	[SerializeField]
	private Transform _transform;

	[SerializeField]
	private MapTileScript[] neighbours;

	void Start()
	{

	}

	public bool CanSee(MapTileScript tile)
	{
		Vector3 diff = tile.GetTransform().position - _transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(_transform.position + new Vector3(0,1,0), diff.normalized, out hitInfo, diff.magnitude, LayerMask.NameToLayer("WorldProj")))
		{
			return false;
		}
		return true;
	}

	public Transform GetTransform()
	{
		return _transform;
	}
}
