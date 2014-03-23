using UnityEngine;
using System.Collections;

/*
 * CharacterAnimatorScript.cs
 * 
 * Receives events from the animator and executes the associated code
 * 
 */

public class CharacterAnimatorScript : MonoBehaviour
{
	private CharacterManager _manager;

	private Animator _animator;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;

		_animator = this.GetComponent<Animator>();
	}

	public Animator GetAnimator()
	{
		return _animator;
	}

	public void StartAttack()
	{
		_animator.speed = _manager.GetStatsScript().GetAttackRate();
	}

	public void DoAttack()
	{
		_manager.GetCombatScript().DoAttack();
	}

	public void EndAttack()
	{ // NOTE: Animations should NOT blend into each other or this is not executed and it makes things look STUPID
		_animator.speed = 1.0f;
	}

	public void UseAbility()
	{

	}

	public void afterDeath()
	{
		_manager.GetCharacterAnimator().SetBool("isDead", false);
	}

	public void afterPain()
	{
		_manager.GetCharacterAnimator().SetBool("onPain", false);
	}
}
