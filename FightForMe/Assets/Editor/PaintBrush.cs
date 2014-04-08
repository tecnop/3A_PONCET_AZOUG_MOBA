using UnityEngine;
using System.Collections;
using UnityEditor;

public class PaintBrush : EditorWindow
{
	private static PaintBrush _brush;

	// Options
	private GameObject prefab;
	private Transform parent;
	private bool symmetry;
	private Transform parent2;

	private PaintBrush()
	{

	}

	public static void Open()
	{
		if (_brush == null)
		{
			_brush = (PaintBrush)CreateInstance("PaintBrush");
		}

		SceneView.onSceneGUIDelegate = _brush.SceneUpdate;

		SceneView.lastActiveSceneView.LookAt(Vector3.zero, Quaternion.Euler(60.0f, 0.0f, 0.0f), 250.0f, true, false);

		_brush.Show();
	}

	void OnGUI()
	{
		this.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab to place", this.prefab, typeof(GameObject), false);

		Transform temp = (Transform)EditorGUILayout.ObjectField("Parent object", this.parent, typeof(Transform), true);

		if (temp != null && PrefabUtility.GetPrefabType(temp) != PrefabType.Prefab)
		{
			this.parent = temp;
		}

		this.symmetry = EditorGUILayout.Toggle("Enable Z axis symmetry?", this.symmetry);

		if (this.symmetry)
		{
			temp = (Transform)EditorGUILayout.ObjectField("Second parent", this.parent2, typeof(Transform), true);

			if (temp != null && PrefabUtility.GetPrefabType(temp) != PrefabType.Prefab)
			{
				this.parent2 = temp;
			}
		}
		else
		{
			this.parent2 = this.parent;
		}
	}

	void SceneUpdate(SceneView scene)
	{
		Event e = Event.current;

		scene.LookAt(Vector3.zero, Quaternion.Euler(60.0f, 0.0f, 0.0f), 250.0f, true, false);

		if (!this.prefab || !e.isMouse)
		{
			return;
		}

		if (e.clickCount > 0 && e.button == 0)
		{
			Ray ray = scene.camera.ScreenPointToRay(e.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1000.0f, (1 << LayerMask.NameToLayer("Terrain"))))
			{ // I have no idea why I have to invert the Z axis, but I don't have enough time to care
				GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(this.prefab);
				obj.transform.position = new Vector3(hitInfo.point.x, 0, -hitInfo.point.z);

				if (this.parent)
				{
					obj.transform.parent = this.parent;
				}

				if (this.symmetry)
				{
					obj = (GameObject)PrefabUtility.InstantiatePrefab(this.prefab);
					obj.transform.position = new Vector3(-hitInfo.point.x, 0, -hitInfo.point.z);

					if (this.parent2)
					{
						obj.transform.parent = this.parent2;
					}
				}
			}
		}
	}
}
