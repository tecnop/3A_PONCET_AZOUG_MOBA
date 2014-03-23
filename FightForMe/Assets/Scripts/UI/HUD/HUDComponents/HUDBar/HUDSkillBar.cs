using UnityEngine;
using System.Collections;

public class HUDSkillBar : HUDComponent
{
	public HUDSkillBar(Rect frame)
		: base("HUD_skill_bar", frame)
	{
		
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;

		GUI.BeginGroup(frame);

		// TODO: Create X buttons with the icon/name of an ability, clicking on them allows the player to change the bound ability

		GUI.EndGroup();
	}
}
