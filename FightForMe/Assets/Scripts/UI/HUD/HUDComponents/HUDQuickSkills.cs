using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDQuickSkills : HUDComponent
{
	PlayerMiscDataScript _misc;
	PlayerInputScript _input;

	public HUDQuickSkills(Rect frame, HUDContainer parent)
		: base("HUD_quick_skills", frame, false, parent)
	{ // Storing it so we don't have to cast it every frame
		_misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
		_input = (PlayerInputScript)GameData.activePlayer.GetInputScript();
	}

	public override void Render()
	{ // TODO: Add a slider to support an unlimited amount of skills
		float w = frame.width;
		float h = frame.height;

		GUI.Box(frame, GUIContent.none);

		GUI.BeginGroup(frame);

		List<Skill> skills = new List<Skill>(_misc.GetAvailableSkills());
		uint i = 0;
		foreach (Skill skill in skills)
		{
			Rect skillRect = new Rect(0.0f, 40.0f * i, w, 40.0f);
			Vector2 absPos = this.GetAbsolutePos() + new Vector2(skillRect.x, skillRect.y);
			Rect absSkillRect = new Rect(absPos.x, absPos.y, skillRect.width, skillRect.height);

			if (GUI.Button(skillRect, skill.GetName()))
			{
				_misc.LearnSkill(DataTables.GetSkillID(skill));
			}
			if (_input.MouseIsInRect(absSkillRect))
			{
				HUDRenderer.SetDataViewObject(skill);
			}
			i++;
		}

		if (GUI.Button(new Rect(0.25f * w, 0.9f * h, 0.5f * w, 0.1f * h), "Fermer"))
		{ // Exit button
			this.enabled = false;
		}

		GUI.EndGroup();
	}
}