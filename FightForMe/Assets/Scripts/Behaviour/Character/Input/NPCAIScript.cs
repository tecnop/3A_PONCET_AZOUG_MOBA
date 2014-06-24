using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AIType { defensive, aggressive, roaming };

public class NPCAIScript : CharacterInputScript
{
	private Transform _transform;

	private MonsterMiscDataScript _misc;

	// AI-Helping variables
	private Vector3 goalPosition;
	private float approachRange;
	private AIType behaviour;
	private bool goalReached;
	private CharacterManager target;
	private Vector3 startPos;
	private AIPath currentPath;
	private Vector3 finalGoalPos;
	private bool targetLost;
	private float searchTime;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = _manager.GetCharacterTransform();
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
		this.startPos = _misc.GetSpawnPos();
		this.currentPath = new AIPath(null);
		this.goalPosition = _transform.position;

		UpdateApproachRange();
	}

	public bool IsSearchingEnemy()
	{
		return !target && behaviour != AIType.defensive;
	}

	public bool HasAnEnemy()
	{
		return target != null;
	}

	public CharacterManager GetEnemy()
	{
		return target;
	}

	public void GainSightOfTarget()
	{
		this.targetLost = false;
		this.searchTime = 0;

		// In case some of them gave up
		SpreadTargetToCamp(this.target);
	}

	public void LoseSightOfTarget()
	{
		this.targetLost = true;
	}

	public void AcknowledgeTarget(CharacterManager target)
	{ // Notice the guy and attack him if we have nothing better to do
		if (!HasAnEnemy() &&
			target.tag == "Player" &&
			target.GetLayer() != _manager.GetLayer())
		{
			Debug.Log(_manager.name + " acquired an enemy: " + target.name);
			SetTarget(target);
		}
		else if (HasAnEnemy() && target == this.target)
		{ // Update the known position
			SetGoal(target.GetCharacterTransform().position);
		}
	}

	public void AcknowledgeTarget(GameObject target)
	{ // Notice the guy and attack him if we have nothing better to do
		if (!HasAnEnemy() &&
			target.tag == "Player" &&
			target.layer != _manager.GetLayer())
		{
			Debug.Log(_manager.name + " acquired an enemy: " + target.name);
			SetTarget(target);
		}
		else if (HasAnEnemy() && target == this.target.gameObject)
		{ // Update the known position
			SetGoal(this.target.GetCharacterTransform().position);
		}
	}

	public void UpdateApproachRange()
	{
		if (!_manager)
		{
			return;
		}

		Weapon weapon = _manager.GetInventoryScript().GetWeapon();
		if (weapon != null)
		{
			WeaponType type = weapon.GetWeaponType();
			if (type != null)
			{
				if (type.IsRanged())
				{
					Projectile proj = weapon.GetProjectile();
					if (proj != null)
					{
						this.approachRange = proj.GetSpeed() / 3.0f;
					}
					else
					{ // Well... it's still a ranged weapon so...
						this.approachRange = 15.0f;
					}
					return;
				}
			}
		}

		this.approachRange = 2.0f;
	}

	public void SpreadTargetToCamp(CharacterManager target)
	{
		List<GameObject> ents = _manager.GetVisionScript().GetEntitiesInSight();
		foreach (GameObject ent in ents)
		{
			CharacterManager hisManager = ent.GetComponent<CharacterManager>();
			if (hisManager != null && hisManager != _manager &&
				hisManager.GetCameraScript() == null &&
				hisManager.GetLayer() == _manager.GetLayer() &&
				Vector3.Distance(_transform.position, hisManager.GetCharacterTransform().position) < 15.0f)
			{ // Those tests are a bit awkward
				NPCAIScript hisAI = ((NPCAIScript)hisManager.GetInputScript());
				if (!hisAI.HasAnEnemy())
				{ // Maybe it should spread again actually?
					hisAI.SetTarget(target, false);
				}
			}
		}
	}

	public void SetTarget(CharacterManager target, bool spread = true)
	{ // TODO: Only the server should do that?
		this.target = target;
		this.targetLost = false;
		this.searchTime = 0;

		if (spread)
		{ // Do the same for everyone in our camp
			SpreadTargetToCamp(target);
		}
	}

	public void SetTarget(GameObject target, bool spread = true)
	{
		CharacterManager hisManager = target.GetComponent<CharacterManager>();
		if (hisManager != null)
		{
			SetTarget(hisManager, spread);
		}
	}

	public void SetGoal(Vector3 pos)
	{
		if (Vector3.Distance(pos, this.finalGoalPos) < 0.5f && !_manager.GetMovementScript().IsMovementOverriden())// && this.currentPath.Count > 1)
		{ // He hasn't moved much and we're already on our way (note: this is making straight movement a bit laggy right now)
			return;
		}

		goalReached = false;

		this.finalGoalPos = pos;

		this.currentPath = AIPath.CreatePath(_transform.position, pos);
		if (this.currentPath.HasNextGoal())
		{
			this.goalPosition = this.currentPath.DequeueNextGoal();
		}
		else
		{
			this.goalPosition = _transform.position;
		}
	}

	private void RunAI()
	{
		if (target)
		{
			if (target.GetStatsScript().GetHealth() <= 0)
			{ // He died
				target = null;
				SetGoal(this.startPos);
			}
			else if (!targetLost)
			{
				SetGoal(target.GetCharacterTransform().position);
			}
		}
		else
		{
			if (this.behaviour == AIType.roaming)
			{ // TODO: Get to a random node
			}
			else
			{
				SetGoal(this.startPos);
			}
		}
	}

	protected override Vector3 UpdateDirectionalInput()
	{
		RunAI();

		if (this.goalReached)
		{
			if (target && targetLost)
			{
				if (searchTime > 0)
				{
					if (Time.time > searchTime)
					{
						Debug.Log(_manager.name + " gave up on searching for " + target.name);
						target = null;
						searchTime = 0;
					}
					else
					{ // Look around?

					}
				}
				else
				{
					searchTime = Time.time + 3.0f;
				}
			}
			return Vector3.zero;
		}

		Vector3 move = this.goalPosition - _transform.position;
		move.y = 0;

		if (move.magnitude < approachRange && this.HasAnEnemy() && !this.currentPath.HasNextGoal())
		{ // If our current goal is our actual target, stay at some distance
			goalReached = true;
			return Vector3.zero;
		}

		if (move.magnitude < 0.25f)
		{
			if (this.currentPath.HasNextGoal())
			{
				this.goalPosition = this.currentPath.DequeueNextGoal();
			}
			else
			{
				this.goalPosition = _transform.position;
				this.goalReached = true;
				return Vector3.zero;
			}
		}

		return move.normalized;
	}

	protected override Vector3 UpdateLookPosition()
	{
		if (Utils.DiffNoY(_transform.position, goalPosition).magnitude < 0.1f)
		{ // Don't panick alright, just look forward
			if (!HasAnEnemy() && _misc.GetSpawner() is MonsterSpawnerScript)
			{ // Unless we have no enemy...
				MonsterSpawnerScript spawner = (MonsterSpawnerScript)_misc.GetSpawner();
				if (spawner)
				{ // And a spawner...
					MonsterCampScript camp = spawner.GetCamp();
					if (camp)
					{ // And a camp!
						return camp.GetPos();
					}
				}
			}

			// TODO: Make the Lord face the direction of his spawner

			return _transform.position + 5.0f * _transform.forward;
		}
		else
		{
			return goalPosition;
		}
	}

	protected override uint UpdateCurrentSpell()
	{
		if (_manager.GetStatsScript().GetHealth() <= 0)
		{ // Can't attack
			return 0;
		}

		if (target && !targetLost && Vector3.Distance(_transform.position, target.GetCharacterTransform().position) <= approachRange * 1.25f)
		{ // Simple attack rule
			return 1;
		}

		return 0;
	}

	protected override void ReadGenericInput()
	{ // Should we do anything here? How about using this to run the AI instead?

	}

	public void SetBehaviour(AIType behaviour)
	{
		this.behaviour = behaviour;
	}
}
