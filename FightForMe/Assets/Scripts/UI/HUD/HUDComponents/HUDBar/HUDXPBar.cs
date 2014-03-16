using UnityEngine;
using System.Collections;

public class HUDXPBar : HUDComponent
{
	public HUDXPBar(Rect frame)
		: base("HUD_exp_bar", frame)
	{

	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(frame);

		uint curXP = ((PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript()).GetExperience();

		// Background
		GUI.Box(localRect, GUIContent.none);

		if (curXP > 0)
		{ // Bar
			GUI.Box(new Rect(0.0f, 0.0f, ((float)curXP / 100.0f) * w, h), GUIContent.none);
		}

		// Text
		GUI.Label(localRect, curXP + " / " + 100 + " XP", FFMStyles.centeredText);

		GUI.EndGroup();
	}
}
