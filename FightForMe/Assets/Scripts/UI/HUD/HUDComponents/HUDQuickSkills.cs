using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDQuickSkills : HUDComponent
{
	PlayerMiscDataScript _misc;
	PlayerInputScript _input;

	Vector2 scrollPos;

	public HUDQuickSkills(Rect frame, HUDContainer parent)
		: base("HUD_quick_skills", frame, false, parent)
	{ // Storing it so we don't have to cast it every frame
		_misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
		_input = (PlayerInputScript)GameData.activePlayer.GetInputScript();
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		//Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "quick_skills_local");
		bool inRect = _input.MouseIsInRect(this.GetAbsoluteRect());

		GUI.Box(frame, GUIContent.none);

		GUI.BeginGroup(frame);

		List<Skill> skills = new List<Skill>(_misc.GetAvailableSkills());
		uint i = 0;

		float scrollHeight = 40.0f * skills.Count;
		if (scrollHeight <= 0.9f * h - 40.0f) scrollHeight = 0.9f * h - 40.0f;
		scrollPos = GUI.BeginScrollView(SRect.Make(0.0f, 0.0f, w, 0.9f * h - 40.0f), scrollPos, SRect.Make(0.0f, 0.0f, w - 20.0f, scrollHeight), false, true);

		foreach (Skill skill in skills)
		{
			Rect skillRect = SRect.Make(0.0f, 40.0f * i, w - 20.0f, 40.0f, "quick_skills_skill" + i);
			Vector2 absPos = this.GetAbsolutePos() + new Vector2(skillRect.x, skillRect.y) - scrollPos;
			Rect absSkillRect = SRect.Make(absPos.x, absPos.y, skillRect.width, skillRect.height, "quick_skills_skill" + i + "_abs", true);

			if (GUI.Button(skillRect, skill.GetName()))
			{
				_misc.LearnSkill(DataTables.GetSkillID(skill));
			}
			if (inRect && _input.MouseIsInRect(absSkillRect))
			{
				HUDRenderer.SetDataViewObject(skill);
			}
			i++;
		}

		GUI.EndScrollView(true);

		if (GUI.Button(SRect.Make(0.0f, 0.9f * h - 35.0f, w, 30.0f, "quick_skills_tree"), "Visualiser l'Arbre"))
		{ // Display the tree
			this.enabled = false;
			HUDRenderer.SetState(HUDState.SkillTree);
		}

		if (GUI.Button(SRect.Make(0.25f * w, 0.9f * h, 0.5f * w, 0.1f * h, "quick_skills_close"), "Fermer"))
		{ // Exit button
			this.enabled = false;
		}

		GUI.EndGroup();
	}
}