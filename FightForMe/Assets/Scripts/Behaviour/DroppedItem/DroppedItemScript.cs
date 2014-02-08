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
		if (Network.isClient || Network.isServer)
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
		if (Vector3.Distance(GameData.activePlayer.GetCharacterTransform().position, _transform.position) < 5.0f)
		{
			PlayerMiscDataScript misc = (PlayerMiscDataScript)GameData.activePlayer.GetMiscDataScript();
			OnRecycle(misc);
		}
	}

	public GraphicsLoader GetGraphicsLoader() { return this._graphics; }
}
