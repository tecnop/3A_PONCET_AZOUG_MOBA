using UnityEngine;
using System.Collections;

public class HUDStatsDisplay : HUDComponent
{
	private bool _advancedStats;

	public HUDStatsDisplay(Rect frame)
		: base("HUD_stats_display", frame)
	{
		_advancedStats = false;
	}

	public override void Render()
	{ // This needs some work, will do for now
		float w = frame.width;
		float h = frame.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(frame);

		if (GUI.Button(localRect, GUIContent.none))
		{
			_advancedStats = !_advancedStats;
		}

		if (!_advancedStats)
		{ // Main stats
			Stats stats = GameData.activePlayer.GetStatsScript().GetStats();
			string statsStr = "Endurance: " + stats.GetStrength() +
				"\nPuissance: " + stats.GetAgility() +
				"\nIntelligence: " + stats.GetIntelligence();

			GUI.Label(localRect, statsStr, FFMStyles.centeredText);
		}
		else
		{ // Misc stats
			string statsStr = "Dégâts: " + GameData.activePlayer.GetStatsScript().GetDamage() +
				"\n" + GameData.activePlayer.GetStatsScript().GetAttackRate() + " attaque(s)/s" +
				"\nVitesse: " + GameData.activePlayer.GetStatsScript().GetMovementSpeed() + " unités/s";

			GUI.Label(localRect, statsStr, FFMStyles.centeredText);
		}

		GUI.EndGroup();
	}
}
