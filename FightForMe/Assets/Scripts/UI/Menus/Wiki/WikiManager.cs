using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WikiState
{
	MAIN_MENU,
	MONSTERS,
	ITEMS,
	WEAPON_TYPES,
	ARMOR_TYPES,
	SKILLS,
	SPELLS,
	BUFFS,
	PROJETILES,
	PAGE			// Entry specific page
}

public static class WikiManager
{
	private static WikiState _state;

	private static WikiEntry _currentEntry;

	public static void DrawWiki()
	{ // TODO: Tabs, state specific drawing, exit button

	}

	private static void DrawMainWikiPage()
	{

	}

	private static void DrawWikiCategory()
	{

	}

	private static void DrawWikiEntry()
	{

	}
}
