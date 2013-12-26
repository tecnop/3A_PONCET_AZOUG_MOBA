using UnityEngine;
using System.Collections;

public class Armor : Item
{
	string name;
	
	string pathModel;
	string pathEffect;
	string pathRarety;
	string pathCapacity;
	string pathIcon;
	//string pathSound;
	
	float scale;
	int recyclingExp;
	int itemLevel;
	// The position of the Armor obj when weared by the player
	uint slot;

	// Test constructor
	public Armor(string name)
	{
		this.name = name;
	}
	
	public Armor(string name,
				string pathModel,
				string pathEffect,
				string pathRarety,
				string pathProjectile,
				string pathCapacity,
				string pathIcon,
				string pathAttackSound,
				float scale,
				int recyclingExp,
				int itemLevel,
				float attackRate,
				float damage,
	            uint slot)
	{
		this.name = name;
		this.pathModel = pathModel;
		this.pathEffect =pathEffect ;
		this.pathRarety = pathRarety;
		this.pathCapacity = pathCapacity;
		this.pathIcon = pathIcon;
		this.scale = scale;
		this.recyclingExp = recyclingExp;
		this.itemLevel = itemLevel;
		this.slot = slot;
	}
	
	public string getName() {return this.name;}
	
	public string getPathModel() {return this.pathModel;}
	public string getPathEffect() {return this.pathEffect;}
	public string getPathRarety() {return this.pathRarety;}
	public string getPathCapacity() {return this.pathCapacity;}
	public string getPathIcon() {return this.pathIcon;}
	
	public float getScale() {return this.scale;}
	public int getRecyclingExp() {return this.recyclingExp;}
	public int getItemLevel() {return this.itemLevel;}
	public uint getSlot() {return this.slot;}
}
