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
		this.AddComponent(new HUDMinimap(SRect.Make()));
		this.AddComponent(new HUDStatsDisplay(SRect.Make(0.025f * w, 0.1f * h, 0.2f * w, 0.8f * h)));
		this.AddComponent(new HUDHealthBar(SRect.Make(0.25f * w, 0.25f * h, 0.5f * w, 0.25f * h)));
		this.AddComponent(new HUDManaBar(SRect.Make(0.25f * w, 0.5f * h, 0.5f * w, 0.25f * h)));
		this.AddComponent(new HUDXPBar(SRect.Make(0.3f * w, 0.75f * h, 0.4f * w, 0.125f * h)));
		this.AddComponent(new HUDMenuButtons(SRect.Make(0.775f * w, 0.1f * h, 0.2f * w, 0.8f * h)));
		*/
		//this.AddComponent(new HUDSpellSlot(SRect.Make(0.575f * w, 0.1f * h, 0.2f * w, 0.8f * h), SpellSlot.SLOT_1));

		new HUDStatsDisplay(SRect.Make(0.025f * w, 0.1f * h, 0.2f * w, 0.8f * h), this);
		new HUDHealthBar(SRect.Make(0.25f * w, 0.05f * h, 0.5f * w, 0.15f * h), this);
		new HUDManaBar(SRect.Make(0.25f * w, 0.2f * h, 0.5f * w, 0.1f * h), this);
		new HUDXPBar(SRect.Make(0.3f * w, 0.3f * h, 0.4f * w, 0.075f * h), this);
		new HUDSpellBar(SRect.Make(0.25f * w, 0.35f * h, 0.5f * w, 0.65f * h), this);
		new HUDMenuButtons(SRect.Make(0.775f * w, 0.1f * h, 0.2f * w, 0.8f * h), this);
	}
}
