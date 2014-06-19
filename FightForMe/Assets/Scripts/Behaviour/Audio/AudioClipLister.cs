using UnityEngine;
using System.Collections;

public class AudioClipLister : MonoBehaviour
{
	[SerializeField]
	private AudioClip[] sounds;

	void Start()
	{
		DataTables.LoadSounds(sounds);
		Destroy(this.gameObject);
	}
}
