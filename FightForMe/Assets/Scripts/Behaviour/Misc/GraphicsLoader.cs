using UnityEngine;
using System.Collections;

public class GraphicsLoader : MonoBehaviour
{
	private Transform _transform;

	private void StoreTransform()
	{ // FIXME FIXME
		if (_transform == null)
			_transform = this.transform;
	}

	private void ClearOldModel()
	{
		foreach (Transform child in _transform)
		{ // Using a loop to get rid of the default model, we should get rid of this later
			Destroy(child.gameObject);
		}
	}

	public void LoadModel(GameObject model, bool safe = true)
	{
		StoreTransform();
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
		StoreTransform();
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
}
