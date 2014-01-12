using UnityEngine;
using System.Collections;

public class CharacterInventoryScript : MonoBehaviour
{
	[SerializeField]
	private GameObject _droppedItemPrefab;

	private Transform _transform;

	private ArrayList items;

	void Start()
	{
		this._transform = this.transform;
		items = new ArrayList();
	}

	public void DropItem(uint index)
	{ // Create a new DroppedItem on a random spot near us FIXME: Don't put it in a wall...
		Vector2 randPos = Random.insideUnitCircle.normalized;
		Vector3 pos = new Vector3(_transform.position.x + randPos.x, _transform.position.y, _transform.position.z + randPos.y);

		GameObject droppedItem = (GameObject)Instantiate(_droppedItemPrefab, pos, Quaternion.identity);
		DroppedItemScript droppedItemScript = droppedItem.GetComponent<DroppedItemScript>();

		droppedItemScript.SetItemID((uint)this.items[(int)index]);
		this.items.RemoveAt((int)index);
	}

	public void DropAllItems()
	{
		for (int i = 0; i < items.Count; i++)
		{
			DropItem(0);
		}
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

		return conflictingItem;
	}
}
