using UnityEngine;
using System.Collections;

public abstract class HUDComponent
{
	protected string name;
	protected Rect frame;

	public bool enabled;

	public HUDComponent(string name, Rect frame, bool enabled = true)
	{
		this.name = name;
		this.frame = frame;
		this.enabled = enabled;
	}

	public abstract void Render();

	public string GetName() { return this.name; }
	public Rect GetFrame() { return this.frame; }
}
