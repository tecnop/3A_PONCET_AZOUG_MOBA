using UnityEngine;
using System.Collections;

public class HUDHealthBar : HUDComponent
{
	public HUDHealthBar(Rect frame, HUDContainer parent)
		: base("HUD_health_bar", frame, parent:parent)
	{

	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(frame);

		float curHealth = GameData.activePlayer.GetStatsScript().GetHealth();
		float maxHealth = GameData.activePlayer.GetStatsScript().GetMaxHealth();

		// Background
		GUI.Box(localRect, GUIContent.none);

		if (curHealth > 1)
		{ // Bar
			GUI.Box(new Rect(0.0f, 0.0f, (curHealth / maxHealth) * w, h), GUIContent.none);
		}

		// Text
		GUIStyle right = FFMStyles.Text(TextAnchor.MiddleRight, rightPadding: 5);
		GUI.Label(localRect, Mathf.Ceil(curHealth) + " / " + Mathf.Ceil(maxHealth) + " HP", FFMStyles.centeredText);
		GUI.Label(localRect, "+ " + GameData.activePlayer.GetStatsScript().GetHealthRegen() + "/s", right);

		GUI.EndGroup();
	}
}
