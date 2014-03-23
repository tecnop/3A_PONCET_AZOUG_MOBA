using UnityEngine;
using System.Collections;

public enum AbilitySlot
{
	SLOT_1,// MOUSE_LEFT,
	SLOT_2,// MOUSE_RIGHT,
	SLOT_3,// MOUSE_MIDDLE,
	SLOT_4,// KEYBOARD_1,
	SLOT_5,// KEYBOARD_2,
	SLOT_6,// KEYBOARD_3,
	SLOT_7,// KEYBOARD_4,
	SLOT_8,// KEYBOARD_R

	NUM_SLOTS
}

public class PlayerMiscDataScript : CharacterMiscDataScript
{
	private ArrayList unlockedSkills;			// Skills we have learned so far (type: Skill)
	private ArrayList availableSkills;			// Neighbours of unlocked skills (type: Skill)
	private uint skillPoints;					// Skill points left to spend
	private uint experience;					// Our current progress to a new skill point

	private uint[] abilitySlots;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;

		unlockedSkills = new ArrayList();
		availableSkills = new ArrayList();

		abilitySlots = new uint[(int)AbilitySlot.NUM_SLOTS];
		for (int i = 0; i < (int)AbilitySlot.NUM_SLOTS; i++)
		{
			abilitySlots[i] = 0;
		}

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

		if (skill.GetEffect() != null && skill.GetEffect().GetUnlockedAbility() != 0)
		{ // TEMPORARY (until we get the skill interface running)
			AssignAbilityToSlot(skill.GetEffect().GetUnlockedAbility(), 1);
		}
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

	public void AssignAbilityToSlot(uint abilityID, int slotID)
	{
		if (slotID < 0 || slotID >= (int)AbilitySlot.NUM_SLOTS)
		{ // Invalid slot
			return;
		}

		if (!_manager.GetStatsScript().GetKnownAbilities().Contains(abilityID))
		{ // Unknown ability
			return;
		}

		abilitySlots[slotID] = abilityID;
	}

	public uint GetAbilityForSlot(int slotID)
	{
		return abilitySlots[slotID];
	}
}
