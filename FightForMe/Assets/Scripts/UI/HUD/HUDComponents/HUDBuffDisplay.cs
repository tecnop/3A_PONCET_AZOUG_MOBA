using UnityEngine;
using System.Collections;

public class HUDBuffDisplay : HUDComponent
{
	public HUDBuffDisplay(Rect frame, HUDContainer parent)
		: base("HUD_buff_display", frame, parent:parent)
	{

	}

	public override void Render()
	{
		GUI.BeginGroup(frame);

		// TODO

		GUI.EndGroup();
	}
}