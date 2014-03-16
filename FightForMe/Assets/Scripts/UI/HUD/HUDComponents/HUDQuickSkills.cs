using UnityEngine;
using System.Collections;

public class HUDQuickSkills : HUDComponent
{
	public HUDQuickSkills(Rect frame)
		: base("HUD_quick_skills", frame, false)
	{
		// TODO: Initialize misc here so we don't have to cast it every frame
	}

	public override void Render()
	{ // TODO: Add a slider to support an unlimited amount of skills
		float w = frame.width;
		float h = frame.height;

		GUI.Box(frame, GUIContent.none);

		GUI.BeginGroup(frame);

		PlayerMiscDataScript misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
		ArrayList objects = new ArrayList(misc.GetAvailableSkills());
		uint i = 0;
		foreach (Skill skill in objects)
		{
			if (GUI.Button(new Rect(0.0f, 40.0f * i, w, 40.0f), skill.GetName()))
			{
				misc.LearnSkill(skill);
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