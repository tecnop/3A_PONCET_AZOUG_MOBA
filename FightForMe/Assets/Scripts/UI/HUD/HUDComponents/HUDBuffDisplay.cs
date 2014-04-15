using UnityEngine;
using System.Collections;

public class HUDBuffDisplay : HUDComponent
{
	private CharacterCombatScript _combat;

	public HUDBuffDisplay(Rect frame, HUDContainer parent)
		: base("HUD_buff_display", frame, parent:parent)
	{
		_combat = GameData.activePlayer.GetCombatScript();
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(frame);

		float x = 0;
		float y = 0;
		float size = 32;

		GUIStyle style = FFMStyles.centeredText;
		style.wordWrap = true;

		foreach(InflictedBuff buff in _combat.GetBuffs())
		{
			GUI.Box(new Rect(x, y, size, size), GUIContent.none);
			GUI.Label(new Rect(x, y, size, size), buff.GetName(), style);

			// TODO: Allow the mouse to hover over the buff to see its description

			x += size;

			if (x + size > localRect.width)
			{
				x = 0;
				y += size;
				if (y + size > localRect.width)
				{
					Debug.LogWarning("WARNING: Too many buffs to fit in the window!");
				}
			}
		}

		GUI.EndGroup();
	}
}