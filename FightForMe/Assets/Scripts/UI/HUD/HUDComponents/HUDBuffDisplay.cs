using UnityEngine;
using System.Collections;

public class HUDBuffDisplay : HUDComponent
{
	private CharacterCombatScript _combat;
	private PlayerInputScript _input;

	public HUDBuffDisplay(Rect frame, HUDContainer parent)
		: base("HUD_buff_display", frame, parent:parent)
	{
		_combat = GameData.activePlayer.GetCombatScript();
		_input = (PlayerInputScript)GameData.activePlayer.GetInputScript();
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
			Rect buffRect = new Rect(x, y, size, size);
			Vector2 absPos = this.GetAbsolutePos() + new Vector2(buffRect.x, buffRect.y);
			Rect absBuffRect = new Rect(absPos.x, absPos.y, buffRect.width, buffRect.height);

			GUI.Box(buffRect, GUIContent.none);
			GUI.Label(buffRect, buff.GetName(), style);

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