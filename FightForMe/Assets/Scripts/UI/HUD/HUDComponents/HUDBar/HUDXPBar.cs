using UnityEngine;
using System.Collections;

public class HUDXPBar : HUDComponent
{
	private PlayerMiscDataScript _misc;

	public HUDXPBar(Rect frame, HUDContainer parent)
		: base("HUD_exp_bar", frame, parent:parent)
	{
		this._misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(frame);

		uint curXP = _misc.GetExperience();

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
