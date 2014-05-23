using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDContainer : HUDComponent
{
	protected List<HUDComponent> children;	// Type: HUDComponent
	protected bool drawBackground;

	public HUDContainer(string name, Rect frame, List<HUDComponent> children = null, bool enabled = true, bool drawBackground = false, HUDContainer parent = null)
		: base(name, frame, enabled, parent)
	{
		if (children != null)
		{
			this.children = new List<HUDComponent>(children);
		}
		else
		{
			this.children = new List<HUDComponent>();
		}

		this.drawBackground = drawBackground;
	}

	public override void Render()
	{
		GUI.BeginGroup(frame);

		if (drawBackground)
		{ // This is terrible
			GUI.Box(SRect.Make(0, 0, frame.width, frame.height), GUIContent.none);
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
		List<HUDComponent> newChildren = new List<HUDComponent>(this.children);
		foreach (HUDComponent component in this.children)
		{
			if (component.GetName() == name)
			{
				newChildren.Remove(component);
			}
		}
		this.children = newChildren;
	}

	public List<HUDComponent> GetChildren()
	{
		return new List<HUDComponent>(this.children);
	}

	public List<HUDComponent> GetChildrenNamed(string name)
	{
		List<HUDComponent> res = new List<HUDComponent>();
		foreach (HUDComponent component in this.children)
		{
			if (component.GetName() == name)
			{
				res.Add(component);
			}
		}
		return res;
	}

	public HUDComponent GetChildNamed(string name)
	{
		foreach (HUDComponent component in this.children)
		{
			if (component.GetName() == name)
			{
				return component;
			}
		}
		return null;
	}
}
