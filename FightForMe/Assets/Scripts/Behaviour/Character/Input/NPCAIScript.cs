﻿using UnityEngine;
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
	private GameObject target;
	private CharacterManager targetManager;
	private Transform targetTransform;
	private Vector3 startPos;
	private Queue<Vector3> currentPath;
	private Vector3 finalGoalPos;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = _manager.GetCharacterTransform();
		_misc = (MonsterMiscDataScript)_manager.GetMiscDataScript();
		this.startPos = _misc.GetSpawnPos();
		this.currentPath = new Queue<Vector3>();
		_networkView = this.GetComponent<NetworkView>();
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

		this.approachRange = 3.0f;
	}

	private void RunAI()
	{
		if (target)
		{
			if (targetManager && targetManager.GetStatsScript().GetHealth() <= 0)
			{ // He died
				target = null;
				SetGoal(this.startPos);
			}
			else
			{
				SetGoal(targetTransform.position);
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

	public void SetTarget(GameObject target, bool spread = true)
	{ // TODO: Only the server should do that?
		this.target = target;
		this.targetTransform = target.transform;
		this.targetManager = target.GetComponent<CharacterManager>();

		if (spread)
		{ // Do the same for everyone in our camp
			List<GameObject> ents = _manager.GetVisionScript().GetEntitiesInSight();
			foreach (GameObject ent in ents)
			{
				CharacterManager hisManager = ent.GetComponent<CharacterManager>();
				if (hisManager != null && hisManager != _manager &&
					hisManager.GetCameraScript() == null &&
					hisManager.GetLayer() == _manager.GetLayer() &&
					Vector3.Distance(_transform.position, hisManager.GetCharacterTransform().position) < 15.0f)
				{
					NPCAIScript hisAI = ((NPCAIScript)hisManager.GetInputScript());
					if (!hisAI.HasAnEnemy())
					{ // Maybe it should spread again actually?
						hisAI.SetTarget(target, false);
					}
				}
			}
		}
	}

	public void SetGoal(Vector3 pos)
	{
		if (Vector3.Distance(pos, this.finalGoalPos) < 1.0f && !_manager.GetMovementScript().IsMovementOverriden())// && this.currentPath.Count > 1)
		{ // He hasn't moved much and we're already on our way (note: this is making straight movement a bit laggy right now)
			return;
		}

		goalReached = false;

		this.finalGoalPos = pos;
		NavMeshPath path = new NavMeshPath();

		if (NavMesh.CalculatePath(_transform.position, pos, (1 << NavMesh.GetNavMeshLayerFromName("Default")), path))
		{
			this.currentPath = new Queue<Vector3>(path.corners);
			this.goalPosition = this.currentPath.Dequeue();
		}
	}

	protected override Vector3 UpdateDirectionalInput()
	{
		RunAI();

		if (this.goalReached)
		{ // This is never true, is it?
			return Vector3.zero;
		}

		Vector3 move = this.goalPosition - _transform.position;
		move.y = 0;

		if (move.magnitude < approachRange && this.HasAnEnemy() && this.currentPath.Count == 0)
		{ // If our current goal is our actual target, stay at some distance
			goalReached = true;
			return Vector3.zero;
		}

		if (move.magnitude < 0.25f)
		{
			if (this.currentPath.Count > 0)
			{
				this.goalPosition = this.currentPath.Dequeue();
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

		if (target && goalReached && Vector3.Distance(_transform.position, targetTransform.position) <= approachRange * 1.1f)
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
