using UnityEngine;
using System.Collections;

public static class Utils
{
	public static Vector3 DiffNoY(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x - b.x, 0, a.z - b.z);
	}

	public static Vector2 MousePosToScreenPos(Vector3 mousePos)
	{
		return new Vector2(mousePos.x, Screen.height - mousePos.y);
	}
}
