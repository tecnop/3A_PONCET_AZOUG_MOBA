using UnityEngine;
using System.Collections;

public class PlayerMiscDataScript : CharacterMiscDataScript
{
	private ArrayList unlockedSkills;			// Skills we have learned so far (type: Skill)
	private ArrayList availableSkills;			// Neighbours of unlocked skills (type: Skill)
	private uint skillPoints;					// Skill points left to spend
	private uint experience;					// Our current progress to a new skill point

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;

		unlockedSkills = new ArrayList();
		availableSkills = new ArrayList();

		skillPoints = 1; // Give us a point
		availableSkills.Add(DataTables.GetSkill(1));	// Force the first skill to be available
		LearnSkill(1); // And learn it
	}

	public void LearnSkill(Skill skill)
	{
		if (skill == null)
		{ // Wat
			return;
		}

		if (skillPoints == 0)
		{ // Not yet!
			return;
		}

		if (!availableSkills.Contains(skill))
		{ // Can't learn it!
			return;
		}

		unlockedSkills.Add(skill);
		availableSkills.Remove(skill);

		ArrayList newSkills = skill.GetNeighbours();
		foreach (Skill newSkill in newSkills)
		{
			if (!unlockedSkills.Contains(newSkill))
			{
				availableSkills.Add(newSkill);
			}
		}

		skillPoints--;

		_manager.GetStatsScript().UpdateStats();
	}

	public void LearnSkill(uint skillID)
	{
		LearnSkill(DataTables.GetSkill(skillID));
	}

	public ArrayList GetUnlockedSkills()
	{
		return this.unlockedSkills;
	}

	public ArrayList GetAvailableSkills()
	{
		return this.availableSkills;
	}

	public uint GetSkillPoints()
	{
		return this.skillPoints;
	}

	public void GainExperience(uint amount)
	{
		this.experience += amount;
		while (experience >= 100)
		{
			experience -= 100;
			skillPoints++;
		}
	}

	public uint GetExperience()
	{
		return this.experience;
	}
}
