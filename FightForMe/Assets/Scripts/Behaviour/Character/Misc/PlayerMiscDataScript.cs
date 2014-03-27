using UnityEngine;
using System.Collections;

public enum SpellSlot
{
	SLOT_0,// MOUSE_LEFT,
	SLOT_1,// MOUSE_RIGHT,
	SLOT_2,// MOUSE_MIDDLE,
	SLOT_3,// KEYBOARD_1,
	SLOT_4,// KEYBOARD_2,
	SLOT_5,// KEYBOARD_3,
	SLOT_6,// KEYBOARD_4,
	SLOT_7,// KEYBOARD_R

	NUM_SLOTS
}

public class PlayerMiscDataScript : CharacterMiscDataScript
{
	private ArrayList unlockedSkills;			// Skills we have learned so far (type: Skill)
	private ArrayList availableSkills;			// Neighbours of unlocked skills (type: Skill)
	private uint skillPoints;					// Skill points left to spend
	private uint experience;					// Our current progress to a new skill point

	private uint[] spellSlots;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;

		unlockedSkills = new ArrayList();
		availableSkills = new ArrayList();

		spellSlots = new uint[(int)SpellSlot.NUM_SLOTS];
		for (int i = 0; i < (int)SpellSlot.NUM_SLOTS; i++)
		{
			spellSlots[i] = 0;
		}

		Skill firstSkill = DataTables.GetSkill(1);
		skillPoints = 1; // Give us a point
		availableSkills.Add(firstSkill);	// Force the first skill to be available
		LearnSkill(firstSkill); // And learn it
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
		{
			AssignSpellToFreeSlot(skill.GetEffect().GetUnlockedAbility());
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

	public void AssignSpellToSlot(uint spellID, SpellSlot slot)
	{
		if (slot < SpellSlot.SLOT_0 || slot >= SpellSlot.NUM_SLOTS)
		{ // Invalid slot (shouldn't really happen)
			return;
		}

		if (spellID > 1 && !_manager.GetStatsScript().GetKnownSpells().Contains(spellID))
		{ // Unknown ability (OR HARD CODED CRAP)
			return;
		}

		spellSlots[(int)slot] = spellID;
	}

	public void AssignSpellToFreeSlot(uint spellID)
	{
		SpellSlot slot = SpellSlot.SLOT_0;

		while (slot < SpellSlot.NUM_SLOTS)
		{
			if (spellSlots[(int)slot] == 0)
			{
				break;
			}
			slot++;
		}

		AssignSpellToSlot(spellID, slot);
	}

	public uint GetSpellForSlot(SpellSlot slot)
	{
		return spellSlots[(int)slot];
	}

	public uint GetSpellForSlot(uint slotID)
	{
		return spellSlots[slotID];
	}
}
