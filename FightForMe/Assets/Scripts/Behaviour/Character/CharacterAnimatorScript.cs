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

	private Transform _transform;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;

		_animator = this.GetComponent<Animator>();
		_transform = _manager.GetCharacterTransform();
	}

	public Animator GetAnimator()
	{
		return _animator;
	}

	public void DoAttack()
	{ // TODO: Check our weapon type and effects
		_manager.GetCombatScript().AreaOfEffect(_transform.position, _transform.rotation, 2.0f, _manager.GetStatsScript().GetDamage());
	}

	public void UseAbility()
	{

	}

	public void isDead()
	{
		print ("X_X");
	}
}
