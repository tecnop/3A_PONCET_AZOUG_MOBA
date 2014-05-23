using UnityEngine;
using System.Collections;

public class HUDBuffDisplay : HUDComponent
{
	private CharacterCombatScript _combat;
	private PlayerInputScript _input;

	public HUDBuffDisplay(Rect frame, HUDContainer parent)
		: base("HUD_buff_display", frame, parent: parent)
	{
		_combat = GameData.activePlayer.GetCombatScript();
		_input = (PlayerInputScript)GameData.activePlayer.GetInputScript();
	}

	private void DrawBuff(Rect frame, string name, float duration = 0)
	{
		GUI.Box(frame, GUIContent.none);
		GUI.Label(frame, name, FFMStyles.centeredText_wrapped);
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "buff_display_local");

		GUI.BeginGroup(frame);

		float x = 0;
		float y = 0;
		float size = 32;

		// FIXME: DIRTY!!!!
		foreach (Buff buff in GameData.activePlayer.GetStatsScript().GetItemBuffs())
		{
			Rect buffRect = SRect.Make(x, y, size, size);
			Vector2 absPos = this.GetAbsolutePos() + new Vector2(buffRect.x, buffRect.y);
			Rect absBuffRect = SRect.Make(absPos.x, absPos.y, buffRect.width, buffRect.height);

			DrawBuff(buffRect, buff.GetName());

			if (_input.MouseIsInRect(absBuffRect))
			{
				HUDRenderer.SetDataViewObject(buff);
			}

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

		foreach (InflictedBuff buff in _combat.GetBuffs())
		{
			Rect buffRect = SRect.Make(x, y, size, size);
			Vector2 absPos = this.GetAbsolutePos() + new Vector2(buffRect.x, buffRect.y);
			Rect absBuffRect = SRect.Make(absPos.x, absPos.y, buffRect.width, buffRect.height);

			DrawBuff(buffRect, buff.GetName());

			if (_input.MouseIsInRect(absBuffRect))
			{
				HUDRenderer.SetDataViewObject(DataTables.GetBuff(buff.GetBuffID()));
			}

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