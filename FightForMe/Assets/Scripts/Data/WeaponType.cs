using UnityEngine;
using System.Collections;

public class WeaponType
{
	private string name;
	private string pathWeaponCollision;
	private string pathAnimationIdle;
	private string pathAnimationAttack;

	// Test constructor
	public WeaponType (string name)
	{
		this.name = name;
	}

	public WeaponType (string name,
	                   string pathWeaponCollion,
	                   string pathAnimationIdle,
	                   string pathAnimationAttack)
	{
		this.name = name;
		this.pathWeaponCollision = pathWeaponCollision;
		this.pathAnimationIdle = pathAnimationIdle;
		this.pathAnimationAttack = pathAnimationAttack;
	} 

	public string getName(){return this.name;}
	public string getPathWeaponCollision(){return this.pathWeaponCollision;}
	public string getAnimationIdle(){return this.pathAnimationIdle;}
	public string getAnimationAttack(){return this.pathAnimationAttack;}

}
