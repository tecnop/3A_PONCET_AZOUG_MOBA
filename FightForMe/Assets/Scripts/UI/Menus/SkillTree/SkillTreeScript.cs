using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SkillTreeScript
{
	private static Vector2 scrollPos;

	private static Rect treeRect;

	private static Dictionary<Skill, Rect> frames;
	private static Dictionary<Skill, List<Vector2>> links;	// Links between neighbours

	private static PlayerMiscDataScript _misc;

	public static void Initialize()
	{
		float minX = -150.0f, minY = -70.0f, maxX = 150.0f, maxY = 70.0f;

		List<Skill> skills = DataTables.GetSkills();
		frames = new Dictionary<Skill, Rect>();
		links = new Dictionary<Skill, List<Vector2>>();

		foreach (Skill skill in skills)
		{ // Parse the list first to get the dimensions of the tree
			Vector2 pos = skill.GetTreePos();
			if (pos.x - 150.0f < minX) minX = pos.x - 150.0f;
			if (pos.x + 150.0f > maxX) maxX = pos.x + 150.0f;
			if (pos.y - 70.0f < minY) minY = pos.y - 70.0f;
			if (pos.y + 70.0f > maxY) maxY = pos.y + 70.0f;
		}

		treeRect = new Rect(0.0f, 0.0f, maxX - minX, maxY - minY);
		
		Vector2 offset = new Vector2(-minX, -minY);

		foreach (Skill skill in skills)
		{ // Now place the skills inside it and create bindings between neighbours
			Vector2 pos = skill.GetTreePos() + offset;
			Rect rect = new Rect(pos.x - 100.0f, pos.y - 20.0f, 200.0f, 40.0f);
			frames.Add(skill, rect);

			foreach (Skill neighbour in skill.GetNeighbours())
			{
				if (links.ContainsKey(neighbour))
				{ // We've drawn his neighbours already, so that probably includes us
					if (!neighbour.GetNeighbours().Contains(skill))
					{ // Or does it? Let's check now
						Debug.LogWarning("One-way relationship detected between skills " + skill.GetName() + " and " + neighbour.GetName());
					}
				}
				else
				{
					if (!links.ContainsKey(skill))
					{
						links.Add(skill, new List<Vector2>());
					}
					links[skill].Add(neighbour.GetTreePos()+offset);
				}
			}
		}

		_misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();

		Vector2 firstPos = skills[0].GetTreePos() + offset;

		int w = Screen.width;
		int h = Screen.height;

		Rect viewRect = SRect.Make(0.0f, 0.095f * h, 0.9f * w, 0.95f * 0.9f * h);
		scrollPos = firstPos - new Vector2(viewRect.width / 2.0f, viewRect.height / 2.0f);
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

		foreach (Skill skill in frames.Keys)
		{
			Rect rect = frames[skill];
			Vector2 pos = new Vector2(rect.x + rect.width / 2.0f, rect.y + rect.height / 2.0f);

			if (links.ContainsKey(skill))
			{
				foreach (Vector2 linkPos in links[skill])
				{ // Draw a line from pos to linkPos (this is very messy)
					// Find the angle between the two positions
					Vector2 diff = linkPos - pos;
					Vector2 center = pos + diff / 2.0f;
					float angle = Mathf.Atan(diff.y / diff.x) * 180.0f / Mathf.PI;
					
					// Build a fake line of just the right size
					int length = Mathf.FloorToInt(diff.magnitude / 4.0f);
					List<string> list = new List<string>(length);
					for (int i = 0; i < length; i++) list.Add("-");

					// And display it (this isn't perfect)
					GUIUtility.RotateAroundPivot(angle, center);
					GUI.Label(new Rect(center.x - 100.0f, center.y - 15.0f, 200.0f, 30.0f), string.Concat(list.ToArray()), FFMStyles.centeredText);
					GUIUtility.RotateAroundPivot(-angle, center);
				}
			}

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

			// TODO: Check for mouse hover events and display the skill's dataview
		}

		GUI.EndScrollView(true);
	}
}
