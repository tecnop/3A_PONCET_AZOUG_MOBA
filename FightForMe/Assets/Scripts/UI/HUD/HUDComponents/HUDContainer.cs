using UnityEngine;
using System.Collections;

public class HUDContainer : HUDComponent
{
	private ArrayList children;	// Type: HUDComponent
	private bool drawBackground;

	public HUDContainer(string name, Rect frame, ArrayList children = null, bool enabled = true, bool drawBackground = false)
		: base(name, frame, enabled)
	{
		if (children != null)
		{
			this.children = new ArrayList(children);
		}
		else
		{
			this.children = new ArrayList();
		}

		this.drawBackground = drawBackground;
	}

	public override void Render()
	{
		GUI.BeginGroup(frame);

		if (drawBackground)
		{ // This is terrible
			GUI.Box(new Rect(0, 0, frame.width, frame.height), GUIContent.none);
		}

		foreach (HUDComponent child in this.children)
		{
			if (child.enabled)
			{
				child.Render();
			}
		}

		GUI.EndGroup();
	}

	public void AddComponent(HUDComponent component)
	{
		this.children.Add(component);
	}

	public void RemoveComponent(HUDComponent component)
	{
		if (this.children.Contains(component))
		{
			this.children.Remove(component);
		}
	}

	public void RemoveComponentsNamed(string name)
	{
		ArrayList newChildren = new ArrayList(this.children);
		foreach (HUDComponent component in this.children)
		{
			if (component.GetName() == name)
			{
				newChildren.Remove(component);
			}
		}
		this.children = newChildren;
	}

	public ArrayList GetChildren()
	{
		return new ArrayList(this.children);
	}

	public ArrayList GetChildrenNamed(string name)
	{
		ArrayList res = new ArrayList();
		foreach (HUDComponent component in this.children)
		{
			if (component.GetName() == name)
			{
				res.Add(component);
			}
		}
		return res;
	}
}
