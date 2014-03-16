using UnityEngine;
using System.Collections;

public class HUDMinimap : HUDComponent
{
	public HUDMinimap(Rect frame)
		: base("HUD_minimap", frame)
	{

	}

	public override void Render()
	{
		GUI.BeginGroup(frame);

		// TODO

		GUI.EndGroup();
	}
}
