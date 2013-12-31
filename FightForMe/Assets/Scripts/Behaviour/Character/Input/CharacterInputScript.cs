using UnityEngine;
using System.Collections;

public abstract class CharacterInputScript : MonoBehaviour
{
	[SerializeField]
	protected CharacterManager _manager;

	public abstract Vector3 GetDirectionalInput();
	public abstract float GetIdealOrientation();
	public abstract void ReadGenericInput();
}
