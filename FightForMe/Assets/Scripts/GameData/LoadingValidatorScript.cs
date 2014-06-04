using UnityEngine;
using System.Collections;

public class LoadingValidatorScript : MonoBehaviour
{
	[SerializeField]
	private NetworkScript _network;

	void Start()
	{
		_network.ValidateLoading();
		Destroy(this.gameObject);
	}
}
