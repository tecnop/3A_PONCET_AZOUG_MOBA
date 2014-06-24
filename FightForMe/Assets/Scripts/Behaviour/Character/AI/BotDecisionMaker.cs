using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotDecisionMaker
{ // A good chunk of this is imported from NPCAIScript.cs, I don't really have enough time to write something clean
	private CharacterManager _manager;
	private PlayerMiscDataScript _misc;
	private Transform _transform;

	private AIPath currentPath;
	private Vector3 currentGoal;

	private Queue<uint> skillPath; // Skills we will get this game

	private CharacterManager enemy;
	private bool enemyLost;
	private float searchTime;
	private float approachRange;

	private DroppedItemScript targetItem; // A nearby item we're targetting

	private Vector3 movement;
	private Vector3 lookPos;
	private uint spell;

	private float decisionTime;

	public BotDecisionMaker(CharacterManager manager)
	{
		_manager = manager;
		_misc = (PlayerMiscDataScript)_manager.GetMiscDataScript();
		_transform = _manager.GetCharacterTransform();

		currentPath = new AIPath(null);

		BuildSkillTree();
	}

	private void BuildSkillTree()
	{ // Decide the skills we're going to get this game
		skillPath = new Queue<uint>(16);
		List<Skill> previousSkills = new List<Skill>();

		Skill currentSkill = DataTables.GetSkill(1);
		previousSkills.Add(currentSkill);

		do
		{
			List<Skill> neighbours = currentSkill.GetNeighbours();
			List<Skill> acceptableNeighbours = new List<Skill>(); // Is that racist? I don't wanna get in trouble please

			foreach (Skill neighbour in neighbours)
			{
				if (!previousSkills.Contains(neighbour))
				{
					acceptableNeighbours.Add(neighbour);
				}
			}

			if (acceptableNeighbours.Count == 0)
			{ // Welp, we're done (now that I think of it, it would probably work better to allow the AI to backtrack a bit instead of creating a single branch)
				break;
			}

			currentSkill = acceptableNeighbours[Random.Range(0, acceptableNeighbours.Count-1)];
			previousSkills.Add(currentSkill);
			skillPath.Enqueue(DataTables.GetSkillID(currentSkill));

		} while (skillPath.Count < 16);
	}

	public bool HasAnEnemy()
	{
		return enemy != null;
	}

	public void GainSightOfTarget()
	{
		this.enemyLost = false;
		this.searchTime = 0;
	}

	public void LoseSightOfTarget()
	{
		this.enemyLost = true;
	}

	public void SetGoal(Vector3 pos)
	{
		this.currentPath = AIPath.CreatePath(_transform.position, pos);
		if (this.currentPath.HasNextGoal())
		{
			this.currentGoal = this.currentPath.DequeueNextGoal();
		}
		else
		{
			this.currentGoal = _transform.position;
		}
	}

	public void SetEnemy(CharacterManager enemy)
	{
		this.enemy = enemy;
		this.enemyLost = false;
		this.searchTime = 0;
	}

	public void SetEnemy(GameObject target)
	{
		CharacterManager hisManager = target.GetComponent<CharacterManager>();
		if (hisManager != null)
		{
			SetEnemy(hisManager);
		}
	}

	public void AcknowledgeTarget(CharacterManager target)
	{ // Notice the guy and attack him if we have nothing better to do
		if (!HasAnEnemy() &&
			target.tag == "Player" &&
			target.GetLayer() != _manager.GetLayer())
		{
			Debug.Log(_manager.name + " acquired an enemy: " + target.name);
			SetEnemy(target);
		}
		else if (HasAnEnemy() && target == this.enemy)
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
			SetEnemy(target);
		}
		else if (HasAnEnemy() && target == this.enemy.gameObject)
		{ // Update the known position
			SetGoal(this.enemy.GetCharacterTransform().position);
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

	private MonsterCampScript ClosestAliveCamp()
	{
		MonsterCampScript closestCamp = null;
		float closestCampDist = 0.0f;

		foreach (MonsterCampScript camp in MonsterCampScript.camps)
		{
			if (!camp.respawning)
			{
				float dist = Vector3.Distance(_transform.position, camp.GetPos());
				if (closestCamp == null || dist < closestCampDist)
				{
					closestCamp = camp;
					closestCampDist = dist;
				}
			}
		}

		return closestCamp;
	}

	public void MakeDecisions()
	{ // Should be called about twice per second at most

		// If we've been attacking someone, keep doing that
		// If we just noticed the enemy player, target him
		// If we're strong enough to go for the objective, do that
		// If we arrived at our destination, engage nearby enemies
		// If we aren't doing anything, target the closest camp

		decisionTime = 0.5f;
	}

	public void RunAI()
	{
		decisionTime -= Time.deltaTime;
		if (decisionTime <= 0.0f)
		{ // When I grow up, I want to be an astronaut
			MakeDecisions();
		}

		if (enemy && enemyLost)
		{
			if (searchTime > 0)
			{
				if (Time.time > searchTime)
				{
					Debug.Log(_manager.name + " gave up on searching for " + enemy.name);
					enemy = null;
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

		Vector3 move = Utils.DiffNoY(this.currentGoal, _transform.position);

		if (move.magnitude < approachRange && this.HasAnEnemy() && !this.currentPath.HasNextGoal())
		{ // If our current goal is our actual target, stay at some distance
			move = Vector3.zero;
		}

		if (move.magnitude < 0.25f)
		{
			if (this.currentPath.HasNextGoal())
			{
				this.currentGoal = this.currentPath.DequeueNextGoal();
			}
			else
			{
				this.currentGoal = _transform.position;
				move = Vector3.zero;
			}
		}

		this.movement = move;
		this.lookPos = _transform.position + move;
		this.spell = 0;
	}

	public Vector3 GetDirectionalInput()
	{
		return movement;
	}

	public Vector3 GetLookPosition()
	{
		return lookPos;
	}

	public uint GetCurrentSpell()
	{
		return spell;
	}
}
