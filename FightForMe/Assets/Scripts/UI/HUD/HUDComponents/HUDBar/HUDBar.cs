using UnityEngine;
using System.Collections;

public class HUDBar : HUDContainer
{
	public HUDBar(Rect frame)
		: base("HUD_bottom_bar", frame, drawBackground:true)
	{
		float w = frame.width;
		float h = frame.height;

		this.AddComponent(new HUDMinimap(new Rect()));
		this.AddComponent(new HUDStatsDisplay(new Rect(0.025f * w, 0.1f * h, 0.2f * w, 0.8f * h)));
		this.AddComponent(new HUDHealthBar(new Rect(0.25f * w, 0.25f * h, 0.5f * w, 0.25f * h)));
		this.AddComponent(new HUDManaBar(new Rect(0.25f * w, 0.5f * h, 0.5f * w, 0.25f * h)));
		this.AddComponent(new HUDXPBar(new Rect(0.3f * w, 0.75f * h, 0.4f * w, 0.125f * h)));
		this.AddComponent(new HUDMenuButtons(new Rect(0.775f * w, 0.1f * h, 0.2f * w, 0.8f * h)));
	}
}
