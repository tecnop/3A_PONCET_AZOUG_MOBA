using UnityEngine;
using System.Collections;

public class HUDDataView : HUDComponent
{

	private WikiEntry currentObject;

	public HUDDataView(Rect frame, HUDContainer parent)
		: base("HUD_data_view", frame, parent: parent)
	{

	}

	public void SetObject(WikiEntry obj)
	{
		this.currentObject = obj;
	}

	public override void Render()
	{
		float w = frame.width;
		float h = frame.height;
		//Rect localRect = new Rect(0.0f, 0.0f, w, h);

		if (currentObject != null)
		{
			this.SetEdgePos(Utils.MousePosToScreenPos(Input.mousePosition));

			GUI.Box(frame, GUIContent.none);

			GUI.BeginGroup(frame);

			currentObject.DrawDataWindow(w, h);

			GUI.EndGroup();
		}
	}
}