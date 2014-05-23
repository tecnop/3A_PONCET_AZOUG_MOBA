using UnityEngine;
using System.Collections;

public class HUDStatsDisplay : HUDComponent
{
	private bool _advancedStats;

	private CharacterStatsScript _stats;

	public HUDStatsDisplay(Rect frame, HUDContainer parent)
		: base("HUD_stats_display", frame, parent: parent)
	{
		_advancedStats = false;
		_stats = GameData.activePlayer.GetStatsScript();
	}

	public override void Render()
	{ // This needs some work, will do for now
		float w = frame.width;
		float h = frame.height;
		Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "stats_local");

		GUI.BeginGroup(frame);

		if (GUI.Button(localRect, GUIContent.none))
		{
			_advancedStats = !_advancedStats;
		}

		if (_advancedStats) // Flipping it for now
		{ // Main stats
			Stats stats = _stats.GetStats();
			string statsStr = "Endurance: " + stats.GetStrength() +
				"\nPuissance: " + stats.GetAgility() +
				"\nIntelligence: " + stats.GetIntelligence();

			GUI.Label(localRect, statsStr, FFMStyles.centeredText);
		}
		else
		{ // Misc stats
			string statsStr = "Dégâts: " + _stats.GetDamage() +
				"\nDégâts à distance: " + _stats.GetProjDamage() +
				"\n" + _stats.GetAttackRate() + " attaque(s)/s" +
				"\nVitesse: " + _stats.GetMovementSpeed() + " unités/s";

			GUI.Label(localRect, statsStr, FFMStyles.centeredText);
		}

		GUI.EndGroup();
	}
}
