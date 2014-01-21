using UnityEngine;
using System.Collections;

public abstract class CharacterMiscDataScript : MonoBehaviour
{
	protected CharacterManager _manager;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
	}
}
