using UnityEngine;
using System.Collections;

public class HUDManaBar : HUDComponent
{
	public HUDManaBar(Rect frame, HUDContainer parent)
		: base("HUD_mana_bar", frame, parent:parent)
	{

	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "mana_bar_local");

		GUI.BeginGroup(frame);

		float curMana = GameData.activePlayer.GetStatsScript().GetMana();
		float maxMana = GameData.activePlayer.GetStatsScript().GetMaxMana();

		// Background
		GUI.Box(localRect, GUIContent.none);

		if (curMana > 1)
		{ // Bar
			GUI.Box(SRect.Make(0.0f, 0.0f, (curMana / maxMana) * w, h), GUIContent.none);
		}

		// Text
		GUIStyle right = FFMStyles.Text(TextAnchor.MiddleRight, rightPadding: 5);
		GUI.Label(localRect, Mathf.Ceil(curMana) + " / " + Mathf.Ceil(maxMana) + " MP", FFMStyles.centeredText);
		GUI.Label(localRect, "+ " + GameData.activePlayer.GetStatsScript().GetManaRegen() + "/s", right);

		GUI.EndGroup();
	}
}
