using UnityEngine;
using System.Collections;

public class HUDBuffDisplay : HUDComponent
{
	public HUDBuffDisplay(Rect frame)
		: base("HUD_buff_display", frame)
	{

	}

	public override void Render()
	{
		GUI.BeginGroup(frame);

		// TODO

		GUI.EndGroup();
	}
}