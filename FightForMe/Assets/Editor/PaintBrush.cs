using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	void OnDestroy()
	{
		SceneView.onSceneGUIDelegate = null;
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

		if (this.prefab)
		{
			if (this.parent && this.symmetry && this.parent2 && this.parent2 != this.parent)
			{
				if (GUILayout.Button("Clean up selected parents"))
				{
					CleanUpParent(this.parent);
					CleanUpParent(this.parent2);
				}
			}
			else if (this.parent || (this.symmetry && this.parent2))
			{
				if (GUILayout.Button("Clean up selected parent"))
				{
					if (this.parent)
					{
						CleanUpParent(this.parent);
					}
					if (this.symmetry && this.parent2 && this.parent2 != this.parent)
					{
						CleanUpParent(this.parent2);
					}
				}
			}
		}
	}

	void CleanUpParent(Transform parent)
	{
		List<Transform> deleteQueue = new List<Transform>();

		foreach (Transform child in parent)
		{
			if (child.gameObject.name == this.prefab.name && // Nasty
				!deleteQueue.Contains(child))
			{
				foreach (Transform other in parent)
				{
					if (child != other &&
						other.gameObject.name == this.prefab.name && // I don't care ok? I can't seem to match an object with its prefab
						!deleteQueue.Contains(other) &&
						Vector3.Distance(child.position, other.position) < 2.0f) // TODO: Change this depending on prefab size
					{
						deleteQueue.Add(other);
					}
				}
			}
		}

		Debug.Log("Cleaning up " + deleteQueue.Count + " objects");

		foreach (Transform child in deleteQueue)
		{
			Undo.DestroyObjectImmediate(child.gameObject);
		}
	}

	void SceneUpdate(SceneView scene)
	{
		Event e = Event.current;

		// Disable selection
		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

		/*if (this.parent)
		{ // LOOK AT IT VERY HARD (doesn't quite work as expected for now)
			Transform trans = this.parent.transform;
			scene.LookAt(trans.position, Quaternion.Euler(60.0f, 0.0f, 0.0f), (trans.localScale.x + trans.localScale.y) * 5.0f, true, false);
		}
		else*/
		{ // Default, hopefully the whole map
			//scene.LookAt(Vector3.zero, Quaternion.Euler(60.0f, 0.0f, 0.0f), 250.0f, true, false);
		}

		if (!this.prefab || !e.isMouse)
		{
			return;
		}

		if (e.type == EventType.MouseDown && // Not accepting MouseDrag, the result might be messy
			e.button == 0 &&
			e.clickCount > 0 &&
			e.clickCount < 10) // Because I can't possibly have clicked 1.5 billion times in one second, thanks Unity
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

				Undo.RegisterCreatedObjectUndo(obj, "Paintbrush placed " + obj.name);

				if (this.symmetry)
				{
					obj = (GameObject)PrefabUtility.InstantiatePrefab(this.prefab);
					obj.transform.position = new Vector3(-hitInfo.point.x, 0, -hitInfo.point.z);

					if (this.parent2)
					{
						obj.transform.parent = this.parent2;
					}

					Undo.RegisterCreatedObjectUndo(obj, "Paintbrush placed " + obj.name + " (symmetry)");
				}
			}
		}
	}
}
