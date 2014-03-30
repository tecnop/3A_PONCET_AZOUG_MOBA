using UnityEngine;
using System.Collections;

public class GraphicsLoader : MonoBehaviour
{
	[SerializeField]
	private Transform _transform;

	private void ClearOldModel()
	{
		foreach (Transform child in _transform)
		{ // Some models may be in multiple parts, we want to clear everything here
			Destroy(child.gameObject);
		}
	}

	public void LoadModel(GameObject model, bool safe = true)
	{
		if (safe && model == null)
		{
			return;
		}

		ClearOldModel();

		if (model != null)
		{
			GameObject newModel = (GameObject)Instantiate(model, _transform.position, _transform.rotation);
			newModel.transform.parent = _transform;
		}
	}

	public void LoadModel(string model, bool safe = true)
	{
		if (model == null)
		{
			if (!safe)
			{
				ClearOldModel();
			}
		}
		else
		{
			LoadModel(DataTables.GetModel(model), safe);
		}
	}

	public Transform GetTransform() { return this._transform; }
}
