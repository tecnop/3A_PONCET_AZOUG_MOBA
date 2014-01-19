using UnityEngine;
using System.Collections;

public class CharacterInventoryScript : MonoBehaviour
{
	[SerializeField]
	private GameObject _droppedItemPrefab;

	[SerializeField]
	private CharacterManager _manager;

	private Transform _transform;

	private ArrayList items;

	void Start()
	{
		this._transform = this.transform;

		if (items == null)
		{
			items = new ArrayList();
		}
	}

	public void DropItem(uint index)
	{ // Create a new DroppedItem on a random spot near us FIXME: Don't put it in a wall...
		Vector2 randPos = Random.insideUnitCircle.normalized;
		Vector3 pos = new Vector3(_transform.position.x + randPos.x, _transform.position.y, _transform.position.z + randPos.y);

		GameObject droppedItem = (GameObject)Instantiate(_droppedItemPrefab, pos, Quaternion.identity);
		DroppedItemScript droppedItemScript = droppedItem.GetComponent<DroppedItemScript>();

		droppedItemScript.SetItemID((uint)this.items[(int)index]);
		this.items.RemoveAt((int)index);

		// Note: Doing that is not ideal when we're dropping all our items, but considering that should only happen when we die, we should be fine
		_manager.GetStatsScript().UpdateStats();
	}

	public void DropAllItems()
	{
		for (int i = 0; i < items.Count; i++)
		{
			Debug.Log("Dropping " + (uint)items[0]);
			DropItem(0);
		}
	}

	public void SetItems(ArrayList items)
	{ // Should only be called when spawning a monster
		this.items = new ArrayList(items);
		//_manager.GetStatsScript().UpdateStats(); // Should we? Need to make sure this is called when I think it is...
	}

	public uint PickUpItem(uint item)
	{
		int i = 0;
		uint conflictingItem = 0; // If we have a weapon currently equipped on that same slot, return it

		Item newItem = DataTables.getItem(item);

		if (newItem == null)
		{ // Hmmm...
			Debug.LogWarning("Player tried to pick up unknown item " + item);
			return 0;
		}

		while (i < items.Count && conflictingItem == 0)
		{
			Item curItem = DataTables.getItem((uint)items[i]);
			if (curItem.isWeapon())
			{ // TODO: Dual-wielding? Yay or nay?
				if (newItem.isWeapon())
				{
					conflictingItem = (uint)items[i];
				}
			}
			else
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
			this.items.Remove(conflictingItem);
			Debug.Log("Player lost item " + conflictingItem + ": " + DataTables.getItem(conflictingItem).getName());
		}

		this.items.Add(item);
		Debug.Log("Player gained item " + item + ": " + DataTables.getItem(item).getName());

		_manager.GetStatsScript().UpdateStats();

		return conflictingItem;
	}
}
