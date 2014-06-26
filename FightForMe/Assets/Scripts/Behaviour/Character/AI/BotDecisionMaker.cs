using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum BotObjectiveType
{
	NONE,
	FARM,		// Engage nearby camps
	FIGHT,		// Engage the other player
	WIN			// Engage the game's objective
}

enum BotSubRoutine
{
	IDLE,
	MOVE,		// Focus on moving to the objective
	FIGHT,		// Focus on engaging the target
	COLLECT,	// Focus on approaching nearby items and deciding what to do with them
	ESCAPE		// Focus on moving away from the target
}

public class BotDecisionMaker
{ // A good chunk of this is imported from NPCAIScript.cs, I don't really have enough time to write something clean
	private CharacterManager _manager;
	private PlayerMiscDataScript _misc;
	private Transform _transform;

	private AIPath currentPath;
	private Vector3 currentDest;

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
	private BotObjectiveType objective;
	private BotSubRoutine routine;
	private Transform goal;
	// Yes, I know all the names above are pretty vague/similar...

	public BotDecisionMaker(CharacterManager manager)
	{
		_manager = manager;
		_misc = (PlayerMiscDataScript)_manager.GetMiscDataScript();
		_transform = _manager.GetCharacterTransform();

		currentPath = new AIPath(null);

		decisionTime = 2.0f;
		objective = BotObjectiveType.NONE;
		routine = BotSubRoutine.IDLE;

		BuildSkillTree();
	}

	private void BuildSkillTree()
	{ // Decide the skills we're going to get this game
		int skillCount = DataTables.GetSkills().Count;
		skillPath = new Queue<uint>(skillCount);
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
			{ // Start backtracking
				int i = previousSkills.Count-1;
				bool found = false;
				while (i >= 0 && !found)
				{
					foreach (Skill neighbour in previousSkills[i].GetNeighbours())
					{
						if (!previousSkills.Contains(neighbour))
						{
							found = true;
							acceptableNeighbours.Add(neighbour);
						}
					}
					i--;
				}

				if (!found)
				{ // We're done, nothing left in the tree (normally this won't happen, we'll have gotten out of the loop first)
					break;
				}
			}

			currentSkill = acceptableNeighbours[Random.Range(0, acceptableNeighbours.Count)];
			previousSkills.Add(currentSkill);
			skillPath.Enqueue(DataTables.GetSkillID(currentSkill));

		} while (skillPath.Count < skillCount);
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

	public void SetDestination(Vector3 pos)
	{
		this.currentPath = AIPath.CreatePath(_transform.position, pos);
		if (this.currentPath.HasNextGoal())
		{
			this.currentDest = this.currentPath.DequeueNextGoal();
		}
		else
		{
			this.currentDest = _transform.position;
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

	public void NoticeEntity(VisibleEntity entity)
	{ // Check out an entity and see if we should do anything with it
		if (entity is DroppedItemScript)
		{
			if (targetItem == null)
			{ // Set it straight away
				targetItem = (DroppedItemScript)entity;
			}
			decisionTime = 0.5f;
		}
		else if (entity is CharacterManager)
		{
			if (!HasAnEnemy() &&
				entity.gameObject.tag == "Player" &&
				entity.gameObject.layer != _manager.GetLayer())
			{ // Set it straight away
				Debug.Log(_manager.name + " acquired an enemy: " + entity.gameObject.name);
				SetEnemy((CharacterManager)entity);
				decisionTime = 0.5f;
			}
			else if (HasAnEnemy() && entity == this.enemy)
			{ // Update the known position
				GainSightOfTarget();
				decisionTime = 0.5f;
			}
		}
	}

	public void ForgetEntity(VisibleEntity entity)
	{
		if (entity == targetItem)
		{ // I guess this is a good place to look for another one?
			targetItem = null;
			decisionTime = 0.5f;
		}
		else if (entity == enemy)
		{
			LoseSightOfTarget();
			decisionTime = 0.5f;
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
	{ // Define our next action

		while (_misc.GetSkillPoints() > 0 && skillPath.Count > 0)
		{ // Got skills to learn
			uint skill = skillPath.Dequeue();
			if (!_misc.LearnSkill(skill))
			{ // Whoops. Queue it up again. This shouldn't happen buuuut safety
				skillPath.Enqueue(skill);
			}
		}

		if (enemy)
		{ // This should always be our main focus
			if (_manager.GetStatsScript().GetHealth() < 0.25f * _manager.GetStatsScript().GetMaxHealth())
			{ // TODO: Take enemy damage into account
				routine = BotSubRoutine.ESCAPE;
			}
			else
			{
				routine = BotSubRoutine.FIGHT;
			}
			goal = enemy.GetCharacterTransform();
		}
		else if (targetItem)
		{ // If we're not engaged in battle, we should interrupt every other routine for this (except maybe when returning an artifact for victory? think about that?)
			routine = BotSubRoutine.COLLECT;
			goal = targetItem.GetTransform();

			if (Utils.DiffNoY(_transform.position, goal.position).magnitude < 3.0f)
			{ // Use it!
				InteractWithItem(targetItem);
			}
		}
		else
		{ // Okay, no nearby priority, let's decide what to do now
			routine = BotSubRoutine.MOVE;

			if ((GameData.gameMode == GameMode.KillTheLord && ((NPCAIScript)LordSpawnerScript.team2Lord.GetInputScript()).GetEnemy() == GameData.activePlayer) ||
				(GameData.gameMode == GameMode.RaceForGlory && GameData.activePlayer.GetStatsScript().HasSpecialEffect(MiscEffect.CARRYING_TROPHY)))
			{ // I want to give him a chance to just decide to attack the other player but can't do for now. Just react to him completing an objective
				objective = BotObjectiveType.FIGHT;

				// Target the other player
				goal = GameData.activePlayer.GetCharacterTransform();
			}
			else if (_misc.GetUnlockedSkills().Count > 5)
			{ // Yes, this is completely arbitrary
				objective = BotObjectiveType.WIN;

				// Target the objective
				if (GameData.gameMode == GameMode.KillTheLord)
				{
					goal = LordSpawnerScript.team1Lord.GetCharacterTransform();
				}
				else if (GameData.gameMode == GameMode.RaceForGlory)
				{
					if (_manager.GetStatsScript().HasSpecialEffect(MiscEffect.CARRYING_TROPHY))
					{
						goal = TrophyDepositAreaScript.team2Area.GetTransform();
					}
					else
					{
						goal = HasnorSpawnerScript.hasnor.GetCharacterTransform();
					}
				}
			}
			else
			{
				objective = BotObjectiveType.FARM;

				// Target the closest active camp
				MonsterCampScript closestCamp = ClosestAliveCamp();
				if (closestCamp)
				{
					goal = ClosestAliveCamp().GetTransform();
				}
				else
				{
					objective = BotObjectiveType.NONE;
					goal = _transform;
					decisionTime = 1.0f;
				}
			}
		}

		if (!enemy)
		{
			RescanSurroundings();
		}

		SetDestination(goal.position);
	}

	public void RescanSurroundings()
	{ // Eeeeeeh
		foreach (VisibleEntity entity in _manager.GetVisionScript().GetEntitiesInSight())
		{
			NoticeEntity(entity);
		}
	}

	public void InteractWithItem(DroppedItemScript droppedItem)
	{
		Item groundItem = DataTables.GetItem(droppedItem.GetItemID());
		Item myItem;
		bool iWantThis = false;

		if (groundItem != null)
		{
			if (groundItem.IsWeapon())
			{
				myItem = _manager.GetInventoryScript().GetWeapon();
			}
			else
			{
				myItem = _manager.GetInventoryScript().GetArmorBySlot(((Armor)groundItem).GetSlot());
			}

			if (myItem == null || groundItem.GetQuality() > myItem.GetQuality())
			{ // A very simple rule, but it works for now
				iWantThis = true;
			}

			if (groundItem.IsWeapon() && iWantThis)
			{
				Weapon groundWeapon = (Weapon)groundItem;
				if (groundWeapon.GetDamage() <= 0.0f && groundWeapon.GetBuff() == null && groundWeapon.GetProjectile() == null)
				{ // Little hack to allow the bot to use the debug XP item. It makes sense anyway, don't want to be wielding an useless weapon.
					iWantThis = false;
				}
			}
		}

		if (iWantThis)
		{
			droppedItem.OnPickUp(_manager.GetInventoryScript());
		}
		else
		{
			droppedItem.OnRecycle(_misc);
		}
	}

	public void RunAI()
	{
		if (decisionTime > 0)
		{
			decisionTime -= Time.deltaTime;
			if (decisionTime <= 0.0f)
			{ // When I grow up, I want to be an astronaut
				MakeDecisions();
			}
		}

		MoveTowardsGoal();

		if (this.routine == BotSubRoutine.FIGHT)
		{
			Fight();
		}
		else
		{
			this.spell = 0;
		}
	}

	private void MoveTowardsGoal()
	{
		Vector3 move = Utils.DiffNoY(this.currentDest, _transform.position);

		if (this.routine == BotSubRoutine.FIGHT)
		{
			if (move.magnitude < approachRange && !this.currentPath.HasNextGoal())
			{ // If our current goal is our actual target, stay at some distance
				move = Vector3.zero;
			}
		}
		else if (this.routine == BotSubRoutine.COLLECT)
		{
			if (move.magnitude < 3.0f && !this.currentPath.HasNextGoal())
			{ // Close enough, interact with it
				if (decisionTime < 0.0f)
				{
					decisionTime = 1.0f;
				}
			}
		}

		if (move.magnitude < 0.25f)
		{
			if (this.currentPath.HasNextGoal())
			{
				this.currentDest = this.currentPath.DequeueNextGoal();
			}
			else
			{
				this.currentDest = _transform.position;
				move = Vector3.zero;

				if (decisionTime < 0)
				{
					decisionTime = 0.5f;
				}
			}
		}

		this.movement = move;

		if (enemy)
		{
			this.lookPos = enemy.GetCharacterTransform().position;
		}
		else if (move.magnitude > 0.1f)
		{
			this.lookPos = _transform.position + move;
		}
		else
		{
			this.lookPos = _transform.position + _transform.rotation * Vector3.forward * 5.0f;
		}
	}

	private void Fight()
	{
		if (!enemy)
		{ // ???
			decisionTime = 0.1f;
			return;
		}

		if (enemyLost)
		{
			this.spell = 0;
			if (searchTime > 0)
			{
				if (Time.time > searchTime)
				{
					Debug.Log(_manager.name + " gave up on searching for " + enemy.name);
					enemy = null;
					searchTime = 0;
					decisionTime = 0.1f;
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
		else if (Utils.DiffNoY(_transform.position, goal.position).magnitude < this.approachRange)
		{ // TODO: Other spells
			this.spell = 1;
		}
		else
		{
			this.spell = 0;
		}
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
