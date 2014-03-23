﻿using UnityEngine;
using System.Collections;

public enum HUDMenu
{
	None,
	Inventory,
	Skill
}

public enum HUDState
{
	Default,
	Won,	// FOR NOW: Player 1 won
	Lost,	// FOR NOW: Player 2 won
	Leaving,	// Asking for confirmation to leave
}

public static class HUDRenderer
{
	private static HUDContainer hudRoot;

	private static HUDState _state;

	private static HUDInventory _inventory;
	private static HUDQuickSkills _skills;

	public static void Initialize()
	{
		float w = Screen.width;
		float h = Screen.height;

		_state = HUDState.Default;

		hudRoot = new HUDContainer("HUD_root", new Rect(0, 0, w, h));

		hudRoot.AddComponent(new HUDBuffDisplay(new Rect()));
		hudRoot.AddComponent(new HUDBar(new Rect(0.0f, 0.8f * h, w, 0.2f * h)));
		_inventory = new HUDInventory(new Rect(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h));
		hudRoot.AddComponent(_inventory);
		_skills = new HUDQuickSkills(new Rect(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h));
		hudRoot.AddComponent(_skills);
	}

	public static void Render()
	{
		float w = Screen.width;
		float h = Screen.height;

		if (GameData.gameType == GameType.ListenServer && Network.connections.Length > 0)
		{ // TEMPORARY
			GUIStyle topRight = FFMStyles.Text(TextAnchor.UpperRight);
			GUI.Label(new Rect(0, 0, w, h), "Ping: " + Network.GetLastPing(Network.connections[0]), topRight);
		}

		// NOTE: This system and layout is temporary!

		if ((_state == HUDState.Default || _state == HUDState.Leaving) && !GameData.gamePaused)
		{
			DrawExitButton();

			if (hudRoot.enabled)
			{
				hudRoot.Render();
			}
		}
		else
		{
			DrawPauseMenu(new Rect(0.25f * w, 0.25f * h, 0.5f * w, 0.5f * h));
		}
	}

	private static void BackToMainMenu()
	{ // This probably shouldn't be here
		if (GameData.isClient)
		{
			Network.Disconnect();
		}
		else if (GameData.isOnline)
		{
			foreach (NetworkPlayer player in Network.connections)
			{
				Network.CloseConnection(player, true);
			}
		}

		Application.LoadLevel(0);
	}

	public static void SetState(HUDState state)
	{ // THIS IS TEMPORARY
		HUDRenderer._state = state;
	}

	public static void SetMenu(HUDMenu menu)
	{ // THIS IS TEMPORARY
		_inventory.enabled = (menu == HUDMenu.Inventory);
		_skills.enabled = (menu == HUDMenu.Skill);
	}

	private static void DrawPauseMenu(Rect rect)
	{ // Game is paused for whatever reason
		float w = rect.width;
		float h = rect.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(rect);

		//GUIStyle bottom = FFMStyles.Text(TextAnchor.LowerCenter, bottomPadding: 2);

		// Background
		GUI.Box(localRect, GUIContent.none);

		if (_state == HUDState.Won)
		{
			GUI.Box(localRect, "Le joueur 1 a gagné!", FFMStyles.centeredText);
		}
		else if (_state == HUDState.Lost)
		{
			GUI.Box(localRect, "Le joueur 2 a gagné!", FFMStyles.centeredText);
		}
		else if (GameData.gamePaused)
		{
			if (GameData.networkError != NetworkConnectionError.NoError)
			{
				GUI.Box(localRect, "En attente de connexion...\n\nErreur: " + GameData.networkError.ToString(), FFMStyles.centeredText);
			}
			else
			{
				GUI.Box(localRect, "En attente de connexion...", FFMStyles.centeredText);
			}
		}

		if (GUI.Button(new Rect(0.4f * w, 0.8f * h, 0.2f * w, 0.2f * h), "Quitter"))
		{
			BackToMainMenu();
		}

		GUI.EndGroup();
	}

	private static void DrawExitButton()
	{
		if (GUI.Button(new Rect(0, 0, 100, 20), "Quitter"))
		{
			SetState(HUDState.Leaving);
		}

		if (_state == HUDState.Leaving)
		{
			if (GUI.Button(new Rect(0, 20, 100, 20), "Confirmer"))
			{
				BackToMainMenu();
			}
			if (GUI.Button(new Rect(100, 20, 100, 20), "Annuler"))
			{
				SetState(HUDState.Default);
			}
		}
	}
}