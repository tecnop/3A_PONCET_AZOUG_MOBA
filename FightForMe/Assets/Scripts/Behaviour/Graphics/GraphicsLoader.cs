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

	public void LoadModel(GameModel model, bool safe = true)
	{
		if (model != null)
		{
			GameObject obj = model.GetModel();

			if (obj || !safe)
			{
				ClearOldModel();

				if (obj)
				{
					GameObject newModel = (GameObject)Instantiate(obj, _transform.position, _transform.rotation);
					newModel.transform.parent = _transform;
				}
			}

			_transform.localScale = new Vector3(model.GetScale(), model.GetScale(), model.GetScale());
		}
		else if (!safe)
		{ // I still don't know what's the point of this
			ClearOldModel();
		}
	}

	/*public void LoadModel(string model, bool safe = true)
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
	}*/

	public Transform GetTransform() { return this._transform; }
}
