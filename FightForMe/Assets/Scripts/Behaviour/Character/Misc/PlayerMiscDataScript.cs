using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private List<Skill> unlockedSkills;			// Skills we have learned so far (type: Skill)
	private List<Skill> availableSkills;			// Neighbours of unlocked skills (type: Skill)
	private uint skillPoints;					// Skill points left to spend
	private uint experience;					// Our current progress to a new skill point

	private uint[] spellSlots;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;

		unlockedSkills = new List<Skill>();
		availableSkills = new List<Skill>();

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

	private bool LearnSkill(Skill skill)
	{
		if (skill == null)
		{ // Wat
			return false;
		}

		if (skillPoints == 0)
		{ // Not yet!
			return false;
		}

		if (!availableSkills.Contains(skill))
		{ // Can't learn it!
			return false;
		}

		unlockedSkills.Add(skill);
		availableSkills.Remove(skill);

		List<Skill> newSkills = skill.GetNeighbours();
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

		return true;
	}

	[RPC]
	public void _LearnSkill(int skillID)
	{
		unlockedSkills.Add(DataTables.GetSkill((uint)skillID));
		_manager.GetStatsScript().UpdateStats();
	}

	public void LearnSkill(uint skillID)
	{
		if (LearnSkill(DataTables.GetSkill(skillID)))
		{
			if (GameData.isOnline)
			{
				_networkView.RPC("_LearnSkill", RPCMode.Others, (int)skillID);
			}
		}
	}

	public List<Skill> GetUnlockedSkills()
	{
		return this.unlockedSkills;
	}

	public List<Skill> GetAvailableSkills()
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
