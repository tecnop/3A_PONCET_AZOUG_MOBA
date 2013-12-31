using UnityEngine;
using System.Collections;

public abstract class CharacterInputScript : MonoBehaviour
{

	public abstract Vector3 GetDirectionalInput();
	public abstract float GetIdealOrientation();

}
