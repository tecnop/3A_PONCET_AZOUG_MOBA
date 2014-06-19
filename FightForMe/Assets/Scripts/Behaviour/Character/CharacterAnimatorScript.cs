using UnityEngine;
using System.Collections;
//using UnityEditorInternal;

/*
 * CharacterAnimatorScript.cs
 * 
 * Receives events from the animator and executes the associated code
 * 
 */

public class CharacterAnimatorScript : MonoBehaviour
{
	private CharacterManager _manager;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private AudioSource _audio;

	/*[SerializeField]
	private AnimatorController _controller;

	private State attackState;*/

	private bool paused;
	private float savedSpeed;

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;

		//_controller = _animator.runtimeAnimatorController as AnimatorController;

		/*int i = 0;
		while (i < _controller.GetLayer(1).stateMachine.stateCount)
		{
			State state = _controller.GetLayer(1).stateMachine.GetState(i);
			if (state.name == "Attack")
			{
				this.attackState = state;
				break;
			}
			i++;
		}

		if (this.attackState == null)
		{
			Debug.LogError("ERROR: Could not find attack state on entity " + _manager.name + "!");
		}*/
	}

	public Animator GetAnimator()
	{
		return _animator;
	}

	public AudioSource GetAudioSource()
	{
		return _audio;
	}

	public void Pause()
	{
		this.paused = true;
		this.savedSpeed = _animator.speed;
		_animator.speed = 0;
	}

	public void Unpause()
	{
		this.paused = false;
		_animator.speed = this.savedSpeed;
	}

	public bool IsPaused()
	{
		return paused;
	}

	public void UpdateAttackRate()
	{
		//this.attackState.speed = _manager.GetStatsScript().GetAttackRate();
	}

	public void StartAttack()
	{
		_manager.GetCharacterAnimator().SetInteger("currentSpell", (int)_manager.GetInputScript().GetCurrentSpell());
		_animator.speed = _manager.GetStatsScript().GetAttackRate();
	}

	public void DoAttack()
	{
		uint spellID = (uint)_manager.GetCharacterAnimator().GetInteger("currentSpell");

		if (!_manager.GetCombatScript().CanUseSpell(spellID))
		{
			return;
		}

		Spell spell = DataTables.GetSpell(spellID);

		_manager.GetCombatScript().UseSpell(spell);

		Utils.PlaySpellSoundOnSource(spell, _audio);
	}

	public void EndAttack()
	{ // FIXME: This is not always executed because animations blend into each other
		_animator.speed = 1.0f;
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
