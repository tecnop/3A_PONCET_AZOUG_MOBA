using UnityEngine;
using System.Collections;

public class DroppedItemScript : MonoBehaviour
{
	[SerializeField] // Serialized for debugging
	private int _itemID; // ...

	[SerializeField]
	private Transform _transform;

	[SerializeField]
	private GraphicsLoader _graphics;

	// Index of the entry in the item table this entity represents
	private uint itemID
	{
		get
		{
			return (uint)_itemID;
		}
		set
		{
			_itemID = (int)value;
		}
	}

	public uint GetItemID()
	{
		return itemID;
	}

	public void SetItemID(uint itemID)
	{
		this.itemID = itemID;
	}

	// Add an "OnClick()" function that displays a menu allowing the user to pick up or recycle the item
	public void OnPickUp(CharacterInventoryScript characterInventory)
	{
		uint conflictingItem = characterInventory.PickUpItem(this.itemID);

		if (conflictingItem == 0)
		{ // No conflict found, he simply took our item, we have no reason to be here anymore
			Destroy(this.gameObject);
			return;
		}

		this.itemID = conflictingItem;
	}

	public void OnRecycle(PlayerMiscDataScript playerMisc)
	{
		playerMisc.GainExperience(DataTables.GetItem(itemID).GetRecyclingXP());
		if (GameData.isOnline)
		{
			Network.Destroy(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	void OnMouseDown()
	{
		HUDRenderer.SetSelectedItem(this);
	}

	public GraphicsLoader GetGraphicsLoader() { return this._graphics; }
	public Transform GetTransform() { return this._transform; }
}
