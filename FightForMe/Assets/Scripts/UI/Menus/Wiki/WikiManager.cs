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

	private static Vector2 scrollPos;

	public static void DrawWiki()
	{
		int w = Screen.width;
		int h = Screen.height;

		Rect wikiRect = SRect.Make(0.05f * w, 0.05f * h, 0.9f * w, 0.9f * h, "wiki_frame");

		GUI.BeginGroup(wikiRect);

		DrawWindow(wikiRect.width, wikiRect.height);

		Rect tabsRect = SRect.Make(0.0f, 0.05f * wikiRect.height, wikiRect.width, 0.15f * wikiRect.height, "wiki_tabs");
		GUI.BeginGroup(tabsRect);
		DrawTabs(wikiRect.width, 0.15f * wikiRect.height);
		GUI.EndGroup();

		if (currentCategory == WikiCategory.NONE)
		{
			Rect mainRect = SRect.Make(0.0f, 0.2f * wikiRect.height, wikiRect.width, 0.8f * wikiRect.height, "wiki_main");
			GUI.BeginGroup(mainRect);
			DrawMainWikiPage(wikiRect.width, 0.8f * wikiRect.height);
			GUI.EndGroup();
		}
		else
		{
			Rect entriesRect = SRect.Make(0.0f, 0.2f * wikiRect.height, 0.2f * wikiRect.width, 0.9f * wikiRect.height, "wiki_entries");
			GUI.BeginGroup(entriesRect);
			DrawCategoryEntries(0.2f * wikiRect.width, 0.8f * wikiRect.height);
			GUI.EndGroup();

			Rect entryRect = SRect.Make(0.2f * wikiRect.width, 0.2f * wikiRect.height, 0.8f * wikiRect.width, 0.8f * wikiRect.height, "wiki_entry");
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
		GUI.Box(SRect.Make(0.0f, 0.0f, width, height, "wiki_window"), GUIContent.none);

		Rect top = SRect.Make(0.0f, 0.0f, width, 0.05f * height, "wiki_top");

		GUI.Box(top, GUIContent.none);
		GUI.Label(top, "Wiki", FFMStyles.centeredText);

		if (GUI.Button(SRect.Make(width - 60, 0.0f, 60, 20, "wiki_close"), "Fermer"))
		{
			HUDRenderer.SetState(HUDState.Default);
		}
	}

	public static string NameForCategory(WikiCategory category)
	{
		if (category == WikiCategory.NONE)
		{
			return "Accueil";
		}
		if (category == WikiCategory.MONSTERS)
		{
			return "Monstres";
		}
		if (category == WikiCategory.ITEMS)
		{
			return "Objets";
		}
		if (category == WikiCategory.WEAPON_TYPES)
		{

			return "Types d'armes";
		}
		if (category == WikiCategory.ARMOR_SETS)
		{
			return "Panoplies";
		}
		if (category == WikiCategory.SKILLS)
		{
			return "Compétences";
		}
		if (category == WikiCategory.SPELLS)
		{
			return "Sorts";
		}
		if (category == WikiCategory.BUFFS)
		{
			return "Buffs";
		}
		if (category == WikiCategory.PROJECTILES)
		{
			return "Projectiles";
		}

		return null;
	}

	private static void SetCategory(WikiCategory category)
	{
		currentCategory = category;
		currentEntry = null;
		scrollPos = Vector2.zero;

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

	public static void SetEntry(WikiEntry entry)
	{
		HUDRenderer.SetState(HUDState.Wiki); // In case it hasn't been done yet!
		SetCategory(entry.category);
		currentEntry = entry;
	}

	public static void DrawReference(WikiEntry target, float x, float y, float width, float height = 20.0f, bool dataView = false)
	{
		if (GUI.Button(SRect.Make(x, y, width, height), target.GetName()))
		{
			SetEntry(target);
		}
	}

	public static void DrawReferenceInLayout(WikiEntry target, bool dataView = false)
	{
		if (GUILayout.Button(target.GetName()))
		{
			SetEntry(target);
		}
	}

	private static void DrawTabs(float width, float height)
	{ // Draw a tab for each category
		float tabWidth = width / (float)WikiCategory.NUM_CATEGORIES;

		for (WikiCategory category = WikiCategory.NONE; category < WikiCategory.NUM_CATEGORIES; category++)
		{
			if (GUI.Button(SRect.Make(tabWidth * (float)category, 0.0f, tabWidth, height, "wiki_tab" + (int)category), NameForCategory(category)))
			{
				SetCategory(category);
			}
		}
	}

	private static void DrawCategoryEntries(float width, float height)
	{ // Draw all existing entries for the current category
		if (displayedEntries != null)
		{
			float scrollHeight = 20.0f * displayedEntries.Count;
			if (scrollHeight <= height) scrollHeight = height;
			scrollPos = GUI.BeginScrollView(SRect.Make(0.0f, 0.0f, width, height), scrollPos, SRect.Make(0.0f, 0.0f, width - 20.0f, scrollHeight), false, true);

			for (int i = 0; i < displayedEntries.Count; i++)
			{
				WikiEntry entry = displayedEntries[i];
				if (GUI.Button(SRect.Make(0.0f, 20.0f * i, width - 20.0f, 20.0f, "wiki_entry" + i), entry.GetName()))
				{
					currentEntry = entry;
				}
			}

			GUI.EndScrollView(true);
		}
		else
		{ // Display something maybe?
			GUI.Label(SRect.Make(0.0f, 0.0f, width, height), "Aucune entrée", FFMStyles.centeredText);
		}
	}

	private static void DrawMainWikiPage(float width, float height)
	{ // No category selected, this is basically a welcome screen
		Rect rect = SRect.Make(0.0f, 0.0f, width, height);
		GUI.Box(rect, GUIContent.none);
		GUI.Label(rect, "Bienvenue dans l'encyclopédie FightForMe!", FFMStyles.centeredText);
	}

	private static void DrawWikiCategory(float width, float height)
	{ // Category selected but no entry yet, here we describe what the current category is about
		Rect rect = SRect.Make(0.0f, 0.0f, width, height);
		GUI.Box(rect, GUIContent.none);
		GUI.Label(rect, "Category (" + currentCategory.ToString() + ")", FFMStyles.centeredText);
	}

	private static void DrawWikiEntry(float width, float height)
	{ // Entry selected (which implies a category), we'll display a category-specific page and fill it with the entry's data
		GUI.Box(SRect.Make(0.0f, 0.0f, width, height), GUIContent.none);
		currentEntry.DrawWikiPage(width, height);
	}
}
