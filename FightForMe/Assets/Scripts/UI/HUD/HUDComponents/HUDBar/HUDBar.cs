using UnityEngine;
using System.Collections;

public class HUDBar : HUDContainer
{
	public HUDBar(Rect frame, HUDContainer parent)
		: base("HUD_bottom_bar", frame, drawBackground:true, parent:parent)
	{
		float w = frame.width;
		float h = frame.height;

		/*
		this.AddComponent(new HUDMinimap(new Rect()));
		this.AddComponent(new HUDStatsDisplay(new Rect(0.025f * w, 0.1f * h, 0.2f * w, 0.8f * h)));
		this.AddComponent(new HUDHealthBar(new Rect(0.25f * w, 0.25f * h, 0.5f * w, 0.25f * h)));
		this.AddComponent(new HUDManaBar(new Rect(0.25f * w, 0.5f * h, 0.5f * w, 0.25f * h)));
		this.AddComponent(new HUDXPBar(new Rect(0.3f * w, 0.75f * h, 0.4f * w, 0.125f * h)));
		this.AddComponent(new HUDMenuButtons(new Rect(0.775f * w, 0.1f * h, 0.2f * w, 0.8f * h)));
		*/
		//this.AddComponent(new HUDSpellSlot(new Rect(0.575f * w, 0.1f * h, 0.2f * w, 0.8f * h), SpellSlot.SLOT_1));

		new HUDStatsDisplay(new Rect(0.025f * w, 0.1f * h, 0.2f * w, 0.8f * h), this);
		new HUDHealthBar(new Rect(0.25f * w, 0.05f * h, 0.5f * w, 0.15f * h), this);
		new HUDManaBar(new Rect(0.25f * w, 0.2f * h, 0.5f * w, 0.1f * h), this);
		new HUDXPBar(new Rect(0.3f * w, 0.3f * h, 0.4f * w, 0.075f * h), this);
		new HUDSpellBar(new Rect(0.25f * w, 0.35f * h, 0.5f * w, 0.65f * h), this);
		new HUDMenuButtons(new Rect(0.775f * w, 0.1f * h, 0.2f * w, 0.8f * h), this);
	}
}
