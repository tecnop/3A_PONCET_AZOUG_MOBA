using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * CharacterInventoryScript.cs
 * 
 * Stores the character's items and acts as an interface to equip/drop items
 * 
 */

public class CharacterInventoryScript : MonoBehaviour
{
	[SerializeField]
	private GameObject _droppedItemPrefab;

	[SerializeField]
	private NetworkView _networkView;

	private CharacterManager _manager;

	private Transform _transform;

	private List<uint> items;					// IDs of the entries in the item table that we are currently carrying (type: uint)
	private Dictionary<uint, uint> setProgress;	// <setID, numItems>
	private List<ArmorSet> completedSets;		// For easier access from other scripts (type: ArmorSet)

	public void Initialize(CharacterManager manager)
	{
		_manager = manager;
		_transform = manager.GetCharacterTransform();

		setProgress = new Dictionary<uint, uint>();
		completedSets = new List<ArmorSet>();
		if (items == null)
		{
			items = new List<uint>();
		}
		else
		{ // We need to update our progress on each set then...
			foreach (uint itemID in items)
			{
				Item item = DataTables.GetItem(itemID);
				if (!item.IsWeapon())
				{
					UpdateSetProgress(((Armor)item).GetSetID(), true);
				}
			}
		}
	}

	private void UpdateSetProgress(uint setID, bool increase)
	{ // NOTE: This method is simple but implies that a character can't have the same item equipped twice
		ArmorSet set = DataTables.GetArmorSet(setID);

		if (set == null)
		{ // Eh, might as well make sure now I guess? This won't (well, shouldn't) be needed for release
			return;
		}

		if (this.setProgress.ContainsKey(setID))
		{
			if (increase)
			{
				this.setProgress[setID]++;
			}
			else
			{
				this.setProgress[setID]--;

				if (this.setProgress[setID] == 0)
				{
					this.setProgress.Remove(setID);
				}
			}
		}
		else
		{
			if (increase)
			{
				this.setProgress.Add(setID, 1);
			}
		}

		// This should do. Not very safe though.
		if (this.setProgress.ContainsKey(setID) &&
			this.setProgress[setID] == set.GetSize())
		{
			this.completedSets.Add(set);
			Debug.Log(_manager.name + " has completed set " + setID);
		}
		else if (this.completedSets.Contains(set))
		{
			this.completedSets.Remove(set);
		}
	}

	public void DropItem(uint index)
	{ // Create a new DroppedItem on a random spot near us FIXME: Don't put it in a wall...
		Vector2 randPos = Random.insideUnitCircle.normalized;
		Vector3 pos = new Vector3(_transform.position.x + randPos.x, _transform.position.y - 1, _transform.position.z + randPos.y);

		GameObject droppedItem;
		if (GameData.isOnline)
		{
			droppedItem = (GameObject)Network.Instantiate(_droppedItemPrefab, pos, Quaternion.identity, 0);
		}
		else
		{
			droppedItem = (GameObject)Instantiate(_droppedItemPrefab, pos, Quaternion.identity);
		}

		DroppedItemScript droppedItemScript = droppedItem.GetComponent<DroppedItemScript>();

		Item item = DataTables.GetItem(this.items[(int)index]);

		if (!item.IsWeapon())
		{ // TODO: This is not pretty. Make this pretty.
			UpdateSetProgress(((Armor)item).GetSetID(), false);
		}

		droppedItemScript.UpdateItemID(this.items[(int)index]);
		this.items.RemoveAt((int)index);

		droppedItemScript.GetGraphicsLoader().LoadModel(item.GetModel());

		// NOTE: Doing that is not ideal when we're dropping all our items, but considering that should only happen when we die, we should be fine
		_manager.GetStatsScript().UpdateStats();
	}

	public void DropAllItems()
	{
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			DropItem(0);
		}
	}

	[RPC]
	private void UpdateItem(int item, bool remove)
	{
		if (remove)
		{
			if (this.items.Contains((uint)item))
			{
				this.items.Remove((uint)item);
			}
			else
			{
				Debug.LogError("Tried to remove item " + item + " from " + _manager.name + " but he doesn't have it!");
			}
		}
		else
		{
			this.items.Add((uint)item);
		}

		_manager.GetStatsScript().UpdateStats();
	}

	public uint PickUpItem(uint item)
	{
		int i = 0;
		uint conflictingItem = 0; // If we have a weapon currently equipped on that same slot, return it

		Item newItem = DataTables.GetItem(item);

		if (newItem == null)
		{ // Hmmm...
			Debug.LogWarning("Player tried to pick up unknown item " + item);
			return 0;
		}

		while (i < items.Count && conflictingItem == 0)
		{
			Item curItem = DataTables.GetItem((uint)items[i]);
			if (curItem.IsWeapon())
			{ // TODO: Dual-wielding? Yay or nay?
				if (newItem.IsWeapon())
				{
					conflictingItem = (uint)items[i];
				}
			}
			else if (!newItem.IsWeapon())
			{
				if (((Armor)newItem).GetSlot() == ((Armor)curItem).GetSlot())
				{
					conflictingItem = (uint)items[i];
				}
			}
			i++;
		}

		if (conflictingItem != 0)
		{
			Item thatItem = DataTables.GetItem(conflictingItem);
			if (!thatItem.IsWeapon())
			{
				UpdateSetProgress(((Armor)thatItem).GetSetID(), false);
			}

			this.items.Remove(conflictingItem);
			Debug.Log(_manager.name + " lost item " + conflictingItem + ": " + DataTables.GetItem(conflictingItem).GetName());

			if (GameData.isOnline)
			{
				_networkView.RPC("UpdateItem", RPCMode.Others, (int)conflictingItem, true);
			}
		}

		this.items.Add(item);
		Debug.Log(_manager.name + " gained item " + item + ": " + DataTables.GetItem(item).GetName());

		if (GameData.isOnline)
		{
			_networkView.RPC("UpdateItem", RPCMode.Others, (int)item, false);
		}

		if (!newItem.IsWeapon())
		{
			UpdateSetProgress(((Armor)newItem).GetSetID(), true);
		}

		_manager.GetStatsScript().UpdateStats();

		return conflictingItem;
	}

	public void SetItems(List<uint> items)
	{ // Should only be called when spawning a monster
		this.items = new List<uint>(items);
		if (_manager)
		{ // We're late!
			foreach (uint itemID in items)
			{
				Item item = DataTables.GetItem(itemID);
				if (!item.IsWeapon())
				{
					UpdateSetProgress(((Armor)item).GetSetID(), true);
				}
			}
		}
	}

	public Weapon GetWeapon()
	{ // NOTE: If we implement dual-wielding, this should be affected // TODO: We implemented dual-wielding, now affect this please
		int i = 0;

		while (i < this.items.Count)
		{
			uint id = (uint)this.items[i];
			Item item = DataTables.GetItem(id);

			if (item != null && item.IsWeapon())
			{
				return (Weapon)item;
			}
			i++;
		}

		return null;
	}

	public Armor GetArmorBySlot(ArmorSlot slot)
	{
		int i = 0;

		while (i < this.items.Count)
		{
			uint id = (uint)this.items[i];
			Item item = DataTables.GetItem(id);

			if (item != null && !item.IsWeapon() && ((Armor)item).GetSlot() == slot)
			{
				return (Armor)item;
			}
			i++;
		}

		return null;
	}

	public List<Armor> GetAllArmor()
	{ // Return type: Armor
		int i = 0;
		List<Armor> res = new List<Armor>();

		while (i < this.items.Count)
		{
			uint id = (uint)this.items[i];
			Item item = DataTables.GetItem(id);

			if (item != null && !item.IsWeapon())
			{
				res.Add((Armor)item);
			}
			i++;
		}

		return res;
	}

	public List<Item> GetItems()
	{ // Return type: Item
		int i = 0;
		List<Item> res = new List<Item>(this.items.Count);

		while (i < this.items.Count)
		{
			uint id = (uint)this.items[i];
			Item item = DataTables.GetItem(id);

			if (item != null)
			{
				res.Add(item);
			}
			i++;
		}

		return res;
	}

	public List<ArmorSet> GetCompletedSets()
	{
		return this.completedSets;
	}
}
