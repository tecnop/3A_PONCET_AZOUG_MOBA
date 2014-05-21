using UnityEngine;
using System.Collections;

public abstract class HUDComponent
{
	protected string name;
	protected Rect frame;

	private HUDContainer parent;

	public bool enabled;

	public HUDComponent(string name, Rect frame, bool enabled = true, HUDContainer parent = null)
	{
		this.name = name;
		this.frame = frame;
		this.enabled = enabled;
		this.parent = parent;
		if (parent != null)
		{
			parent.AddComponent(this);
		}
	}

	public abstract void Render();

	public string GetName() { return this.name; }
	public Rect GetFrame() { return this.frame; }

	public Vector2 GetAbsolutePos()
	{
		Vector2 myPos = new Vector2(this.frame.x, this.frame.y);

		if (parent != null)
		{
			myPos += parent.GetAbsolutePos();
		}

		return myPos;
	}

	public Rect GetAbsoluteRect()
	{
		Vector2 pos = GetAbsolutePos();

		return new Rect(pos.x, pos.y, frame.width, frame.height);
	}

	internal void SetPos(float x, float y)
	{
		if (x < 0)
		{
			x = 0;
		}
		else if (x + frame.width > Screen.width)
		{
			x = Screen.width - frame.width;
		}

		if (y < 0)
		{
			y = 0;
		}
		else if (y + frame.height > Screen.height)
		{
			y = Screen.height - frame.height;
		}

		this.frame = new Rect(x, y, frame.width, frame.height);
	}

	internal void SetPos(Vector2 pos)
	{
		SetPos(pos.x, pos.y);
	}

	internal void SetEdgePos(float x, float y)
	{
		if (x < 0)
		{
			x = 0;
		}
		else if (x + frame.width > Screen.width)
		{
			x -= frame.width;
		}

		if (y < 0)
		{
			y = 0;
		}
		else if (y + frame.height > Screen.height)
		{
			y -= frame.height;
		}

		this.frame = new Rect(x, y, frame.width, frame.height);
	}

	internal void SetEdgePos(Vector2 pos)
	{
		SetEdgePos(pos.x, pos.y);
	}

	public HUDContainer GetParent() { return this.parent; }
}
