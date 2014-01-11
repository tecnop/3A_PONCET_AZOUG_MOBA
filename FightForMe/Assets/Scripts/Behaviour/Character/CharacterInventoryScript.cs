using UnityEngine;
using System.Collections;

public class CharacterInventoryScript : MonoBehaviour
{
	public ArrayList items;

	public void DropItem(uint index)
	{

	}

	public uint PickUpItem(uint item)
	{
		uint conflictingItem = 0; // If we have a weapon currently equipped on that same slot, return it

		// Here: foreach in items, check the slot of each item and find one conflicting
		// Once we find it, set conflictingItem to its index value

		this.items.Add(item);

		return conflictingItem;
	}
}
