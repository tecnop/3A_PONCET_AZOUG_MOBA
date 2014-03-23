using UnityEngine;
using System.Collections;

public class HUDQuickSkills : HUDComponent
{
	PlayerMiscDataScript _misc;

	public HUDQuickSkills(Rect frame)
		: base("HUD_quick_skills", frame, false)
	{ // Storing it so we don't have to cast it every frame
		_misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
	}

	public override void Render()
	{ // TODO: Add a slider to support an unlimited amount of skills
		float w = frame.width;
		float h = frame.height;

		GUI.Box(frame, GUIContent.none);

		GUI.BeginGroup(frame);

		ArrayList objects = new ArrayList(_misc.GetAvailableSkills());
		uint i = 0;
		foreach (Skill skill in objects)
		{
			if (GUI.Button(new Rect(0.0f, 40.0f * i, w, 40.0f), skill.GetName()))
			{
				_misc.LearnSkill(skill);
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