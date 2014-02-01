using UnityEngine;
using System.Collections;

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

	public void DoAttack()
	{
		_manager.GetCombatScript().AreaOfEffect(_manager.GetCharacterTransform().position, 2.0f, 10.0f);
	}

	public void DoSkill()
	{

	}
}
