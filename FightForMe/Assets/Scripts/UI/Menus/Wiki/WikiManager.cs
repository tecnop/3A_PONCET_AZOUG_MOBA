using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WikiCategory
{
	NONE,	// AKA Main menu
	MONSTERS,
	ITEMS,
	WEAPON_TYPES,
	ARMOR_SETS,
	SKILLS,
	SPELLS,
	BUFFS,
	PROJECTILES,

	NUM_CATEGORIES
}

public static class WikiManager
{
	private static WikiCategory currentCategory;

	private static List<WikiEntry> displayedEntries;	// All entries of the current category

	private static WikiEntry currentEntry;

	public static void DrawWiki()
	{
		int w = Screen.width;
		int h = Screen.height;

		Rect wikiRect = new Rect(0.05f * w, 0.05f * h, 0.9f * w, 0.9f * h);

		GUI.BeginGroup(wikiRect);

		DrawWindow(wikiRect.width, wikiRect.height);

		Rect tabsRect = new Rect(0.0f, 0.05f * wikiRect.height, wikiRect.width, 0.15f * wikiRect.height);
		GUI.BeginGroup(tabsRect);
		DrawTabs(wikiRect.width, 0.15f * wikiRect.height);
		GUI.EndGroup();

		if (currentCategory == WikiCategory.NONE)
		{
			Rect mainRect = new Rect(0.0f, 0.2f * wikiRect.height, wikiRect.width, 0.8f * wikiRect.height);
			GUI.BeginGroup(mainRect);
			DrawMainWikiPage(wikiRect.width, 0.8f * wikiRect.height);
			GUI.EndGroup();
		}
		else
		{
			Rect entriesRect = new Rect(0.0f, 0.2f * wikiRect.height, 0.2f * wikiRect.width, 0.9f * wikiRect.height);
			GUI.BeginGroup(entriesRect);
			DrawCategoryEntries(0.2f * wikiRect.width, 0.9f * wikiRect.height);
			GUI.EndGroup();

			Rect entryRect = new Rect(0.2f * wikiRect.width, 0.2f * wikiRect.height, 0.8f * wikiRect.width, 0.8f * wikiRect.height);
			GUI.BeginGroup(entryRect);
			if (currentEntry != null)
			{
				DrawWikiEntry(0.8f * wikiRect.width, 0.8f * wikiRect.height);
			}
			else
			{
				DrawWikiCategory(0.8f * wikiRect.width, 0.8f * wikiRect.height);
			}
			GUI.EndGroup();
		}

		GUI.EndGroup();
	}

	private static void DrawWindow(float width, float height)
	{// Main Window
		GUI.Box(new Rect(0.0f, 0.0f, width, height), GUIContent.none);

		GUI.Box(new Rect(0.0f, 0.0f, width, 0.05f * height), GUIContent.none);
		GUI.Label(new Rect(0.0f, 0.0f, width, 0.05f * height), "Wiki", FFMStyles.centeredText);

		if (GUI.Button(new Rect(width - 60, 0.0f, 60, 20), "Fermer"))
		{
			HUDRenderer.SetState(HUDState.Default);
		}
	}

	private static void SetCategory(WikiCategory category)
	{
		currentCategory = category;
		currentEntry = null;

		displayedEntries = new List<WikiEntry>();

		if (category == WikiCategory.MONSTERS)
			foreach (Monster monster in DataTables.GetMonsters())
				displayedEntries.Add(monster);
		else if (category == WikiCategory.ITEMS)
			foreach (Item item in DataTables.GetItems())
				displayedEntries.Add(item);
		else if (category == WikiCategory.WEAPON_TYPES)
			foreach (WeaponType weaponType in DataTables.GetWeaponTypes())
				displayedEntries.Add(weaponType);
		else if (category == WikiCategory.ARMOR_SETS)
			foreach (ArmorSet armorSet in DataTables.GetArmorSets())
				displayedEntries.Add(armorSet);
		else if (category == WikiCategory.SKILLS)
			foreach (Skill skill in DataTables.GetSkills())
				displayedEntries.Add(skill);
		else if (category == WikiCategory.SPELLS)
			foreach (Spell spell in DataTables.GetSpells())
				displayedEntries.Add(spell);
		else if (category == WikiCategory.BUFFS)
			foreach (Buff buff in DataTables.GetBuffs())
				displayedEntries.Add(buff);
		else if (category == WikiCategory.PROJECTILES)
			foreach (Projectile projectile in DataTables.GetProjectiles())
				displayedEntries.Add(projectile);
	}

	private static void DrawTabs(float width, float height)
	{ // Draw a tab for each category
		float tabWidth = width / (float)WikiCategory.NUM_CATEGORIES;

		for (WikiCategory category = WikiCategory.NONE; category < WikiCategory.NUM_CATEGORIES; category++)
		{
			if (GUI.Button(new Rect(tabWidth * (float)category, 0.0f, tabWidth, height), category.ToString()))
			{
				SetCategory(category);
			}
		}
	}

	private static void DrawCategoryEntries(float width, float height)
	{ // Draw all existing entries for the current category
		if (displayedEntries != null)
		{
			for (int i = 0; i < displayedEntries.Count; i++)
			{
				WikiEntry entry = displayedEntries[i];
				if (GUI.Button(new Rect(0.0f, 20.0f * i, width, 20.0f), entry.GetName()))
				{
					currentEntry = entry;
				}
			}
		}
		else
		{ // Display something maybe?
			GUI.Label(new Rect(0.0f, 0.0f, width, height), "Aucune entrée", FFMStyles.centeredText);
		}
	}

	private static void DrawMainWikiPage(float width, float height)
	{ // No category selected, this is basically a welcome screen
		GUI.Box(new Rect(0.0f, 0.0f, width, height), GUIContent.none);
		GUI.Label(new Rect(0.0f, 0.0f, width, height), "Bienvenue dans l'encyclopédie FightForMe!", FFMStyles.centeredText);
	}

	private static void DrawWikiCategory(float width, float height)
	{ // Category selected but no entry yet, here we describe what the current category is about
		GUI.Box(new Rect(0.0f, 0.0f, width, height), GUIContent.none);
		GUI.Label(new Rect(0.0f, 0.0f, width, height), "Category (" + currentCategory.ToString() + ")", FFMStyles.centeredText);
	}

	private static void DrawWikiEntry(float width, float height)
	{ // Entry selected (which implies a category), we'll display a category-specific page and fill it with the entry's data
		GUI.Box(new Rect(0.0f, 0.0f, width, height), GUIContent.none);
		currentEntry.DrawWikiPage(width, height);
	}
}
