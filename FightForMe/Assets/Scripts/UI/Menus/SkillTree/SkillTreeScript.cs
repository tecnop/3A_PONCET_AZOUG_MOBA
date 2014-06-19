using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SkillTreeScript
{
	private static Vector2 scrollPos;

	private static Rect treeRect;

	private static List<Skill> skills;

	private static PlayerMiscDataScript _misc;

	private static Vector2 offset;

	public static void Initialize()
	{ // Get the size of the tree's view rect
		float minX = -150.0f, minY = -70.0f, maxX = 150.0f, maxY = 70.0f;

		skills = DataTables.GetSkills();

		foreach (Skill skill in skills)
		{
			Vector2 pos = skill.GetTreePos();
			if (pos.x - 150.0f < minX) minX = pos.x - 150.0f;
			if (pos.x + 150.0f > maxX) maxX = pos.x + 150.0f;
			if (pos.y - 70.0f < minY) minY = pos.y - 70.0f;
			if (pos.y + 70.0f > maxY) maxY = pos.y + 70.0f;
		}

		treeRect = new Rect(0.0f, 0.0f, maxX - minX, maxY - minY);
		offset = new Vector2(-minX, -minY);

		_misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();

		// TODO: Build the frames and relations right here
	}

	public static void DrawSkillTree()
	{
		int w = Screen.width;
		int h = Screen.height;

		Rect windowRect = SRect.Make(0.05f * w, 0.05f * h, 0.9f * w, 0.9f * h, "skilltree_frame");

		GUI.BeginGroup(windowRect);

		DrawWindow(windowRect.width, windowRect.height);

		Rect treeRect = SRect.Make(0.0f, 0.05f * windowRect.height, windowRect.width, 0.95f * windowRect.height, "skilltree_tree");

		GUI.BeginGroup(treeRect);

		DrawTree(treeRect.width, treeRect.height);

		GUI.EndGroup();

		GUI.EndGroup();
	}

	private static void DrawWindow(float width, float height)
	{// Main Window
		GUI.Box(SRect.Make(0.0f, 0.0f, width, height, "skilltree_window"), GUIContent.none);

		Rect top = SRect.Make(0.0f, 0.0f, width, 0.05f * height, "skilltree_top");

		GUI.Box(top, GUIContent.none);
		GUI.Label(top, "Arbre des compétences", FFMStyles.centeredText);

		if (GUI.Button(SRect.Make(width - 60, 0.0f, 60, 20, "skilltree_close"), "Fermer"))
		{
			HUDRenderer.SetState(HUDState.Default);
		}
	}

	private static void DrawTree(float width, float height)
	{
		scrollPos = GUI.BeginScrollView(SRect.Make(0.0f, 0.0f, width, height, "skilltree_tree_local"), scrollPos, treeRect);

		List<Skill> availableSkills = _misc.GetAvailableSkills();

		foreach (Skill skill in skills)
		{
			Vector2 pos = skill.GetTreePos();
			Rect rect = new Rect(offset.x + pos.x - 100.0f, offset.y + pos.y - 20.0f, 200.0f, 40.0f);

			if (availableSkills.Contains(skill))
			{
				if (GUI.Button(rect, skill.GetName()))
				{
					_misc.LearnSkill(DataTables.GetSkillID(skill));
				}
			}
			else
			{
				GUI.Box(rect, GUIContent.none);
				GUI.Label(rect, skill.GetName(), FFMStyles.centeredText);
			}
		}

		GUI.EndScrollView(true);
	}
}
