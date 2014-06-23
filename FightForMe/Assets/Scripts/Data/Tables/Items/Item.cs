using UnityEngine;
using System.Collections;

public abstract class Item : WikiEntry
{
	protected uint recyclingXP;		// Experience reward for recycling this item
	protected uint buffID;			// ID of the entry in the buff table carrying this item enables

	private bool isWeapon;			// True if the item is a weapon, false if it is an armor

	protected Item(Metadata metadata,
		uint recyclingXP,
		uint buffID,
		bool isWeapon)
		: base(metadata)
	{
		this.recyclingXP = recyclingXP;
		this.buffID = buffID;
		this.isWeapon = isWeapon;
	}

	public uint GetRecyclingXP()
	{
		return this.recyclingXP;
	}

	public Buff GetBuff()
	{
		return DataTables.GetBuff(this.buffID);
	}

	public bool IsWeapon()
	{
		return this.isWeapon;
	}

	public override void DrawDataWindow(float width, float height)
	{
		base.DrawDataWindow(width, height);

		if (this.buffID != 0)
		{
			GUI.BeginGroup(SRect.Make(0.0f, height - 40.0f, width, 20.0f, "data_window_item_buff"));
			GUILayout.BeginHorizontal();
			GUILayout.Label("Applique l'effet");
			WikiManager.DrawReferenceInLayout(this.GetBuff());
			GUILayout.Label("au porteur");
			GUILayout.EndHorizontal();
			GUI.EndGroup();
		}

		GUI.Label(SRect.Make(0.7f * width, height - 20.0f, 0.3f * width, 20.0f, "data_window_item_recycle"), "Recyclage: " + this.recyclingXP + " XP", FFMStyles.centeredText_wrapped);
	}

	public override void DrawWikiPage(float width, float height)
	{
		base.DrawWikiPage(width, height);
	}
}
