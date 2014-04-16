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

	[SerializeField]
	private NetworkView _networkView;

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

	[RPC]
	private void _UpdateItemID(int itemID)
	{ // Server only
		if (itemID == 0)
		{ // No conflict found, he simply took our item, we have no reason to be here anymore
			if (GameData.isOnline)
			{
				Network.Destroy(this.gameObject);
			}
			else
			{
				Destroy(this.gameObject);
			}
			return;
		}

		_networkView.RPC("SetItemID", RPCMode.All, itemID);
	}

	[RPC]
	private void SetItemID(int itemID)
	{
		this.itemID = (uint)itemID;
	}

	public void UpdateItemID(uint itemID)
	{
		if (GameData.isOnline)
		{
			_networkView.RPC("_UpdateItemID", RPCMode.Server, (int)itemID);
		}
		else
		{
			this._UpdateItemID((int)itemID);
		}
	}

	public void OnPickUp(CharacterInventoryScript characterInventory)
	{
		uint conflictingItem = characterInventory.PickUpItem(this.itemID);

		UpdateItemID(conflictingItem);
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
