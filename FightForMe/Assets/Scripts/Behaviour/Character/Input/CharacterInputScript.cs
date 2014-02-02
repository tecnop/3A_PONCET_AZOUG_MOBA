using UnityEngine;
using System.Collections;

/*
 * CharacterInputScript.cs
 * 
 * Reads the character's input
 * 
 */

public abstract class CharacterInputScript : MonoBehaviour
{
	protected CharacterManager _manager;

	public abstract void Initialize(CharacterManager manager);

	public abstract Vector3 GetDirectionalInput();
	public abstract float GetIdealOrientation();
	public abstract void ReadGenericInput();
}
