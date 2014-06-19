using UnityEngine;
using System.Collections;

public class DroppedItemScript : MonoBehaviour//NetworkedEntityScript
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
				_networkView.RPC("KillMe", RPCMode.Server);
			}
			else
			{
				Destroy(this.gameObject);
			}
			return;
		}

		if (GameData.isOnline)
		{
			_networkView.RPC("SetItemID", RPCMode.AllBuffered, itemID);
		}
		else
		{
			SetItemID(itemID);
		}
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
		Item item = DataTables.GetItem(itemID);

		if (item != null)
		{
			playerMisc.GainExperience(item.GetRecyclingXP());
		}

		if (GameData.isOnline)
		{
			_networkView.RPC("KillMe", RPCMode.Server);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	[RPC]
	private void KillMe()
	{
		_networkView.RPC("DoKill", RPCMode.AllBuffered);
	}

	[RPC]
	private void DoKill()
	{
		//Network.Destroy(this.gameObject);
		Destroy(this.gameObject);
	}

	void OnMouseDown()
	{
		HUDRenderer.SetSelectedItem(this);
	}

	void OnGUI()
	{
		if (!_graphics.gameObject.activeSelf || HUDRenderer.GetState() != HUDState.Default)
		{
			return;
		}

		Item item = DataTables.GetItem(this.itemID); // Don't do this every frame please
		if (item != null)
		{ // FIXME: Getting some weird errors from the camera here apparently
			Vector3 screenPos = GameData.activePlayer.GetCameraScript().GetCamera().WorldToScreenPoint(_transform.position + new Vector3(0, 3, 0));

			float w = 150, h = 100;

			GUI.BeginGroup(SRect.Make(screenPos.x - w / 2.0f, Screen.height - screenPos.y - h / 2.0f, w, h));
			string name = item.GetName();

			GUI.Label(SRect.Make(0, 0, w, h, "dropped_item_name"), name, FFMStyles.centeredText_wrapped);

			GUI.EndGroup();
		}
	}

	public GraphicsLoader GetGraphicsLoader() { return this._graphics; }
	public Transform GetTransform() { return this._transform; }
}
