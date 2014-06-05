using UnityEngine;
using System.Collections;

public class LoadingValidatorScript : MonoBehaviour
{
	[SerializeField]
	private NetworkScript _network;

	void Start()
	{
		if (GameData.isOnline)
		{
			_network.ValidateLoading();
		}
		Destroy(this.gameObject);
	}
}
