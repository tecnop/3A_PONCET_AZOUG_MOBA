using UnityEngine;
using System.Collections;

public enum HUDMenu
{
	None = 0,
	Inventory = 1,
	QuickSkill = 2,
	SpellSlot = 4,
	SkillTree = 8
}

public enum HUDState
{
	Default,	// Showing the HUD
	Wiki,		// Showing the wiki
	SkillTree	// Showing the skill tree
}

public static class HUDRenderer
{
	private static HUDContainer hudRoot;

	private static HUDState _state;
	private static SpellSlot _activeSlot;

	// Special components
	private static HUDInventory _inventory;
	private static HUDQuickSkills _skills;
	private static HUDSpellWindow _spells;
	private static HUDDroppedItemWindow _droppedItem;
	private static HUDDataView _dataView;

	private static DroppedItemScript _selectedItem;

	private static bool isLeaving;

	public static void Initialize()
	{
		float w = Screen.width;
		float h = Screen.height;

		FFMStyles.Load();

		SkillTreeScript.Initialize();

		_state = HUDState.Default;
		_activeSlot = SpellSlot.NUM_SLOTS;

		hudRoot = new HUDContainer("HUD_root", SRect.Make(0, 0, w, h));

		new HUDBuffDisplay(SRect.Make(0.4f * w, 0.7f * h, 0.2f * w, 0.1f * h), hudRoot);
		new HUDBar(SRect.Make(0.0f, 0.8f * h, w, 0.2f * h), hudRoot);
		_inventory = new HUDInventory(SRect.Make(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h), hudRoot);
		_skills = new HUDQuickSkills(SRect.Make(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h), hudRoot);
		_spells = new HUDSpellWindow(SRect.Make(0.3f * w, 0.5f * h, 0.4f * w, 0.3f * h), hudRoot);
		_droppedItem = new HUDDroppedItemWindow(SRect.Make(0.3f * w, 0.5f * h, 0.4f * w, 0.3f * h), hudRoot);
		_dataView = new HUDDataView(SRect.Make(0.0f, 0.4f * h, 0.35f * w, 0.3f * h), hudRoot);
	}

	public static void Render()
	{
		float w = Screen.width;
		float h = Screen.height;

		if (GameData.gameType == GameType.ListenServer && Network.connections.Length > 0)
		{ // TEMPORARY
			GUIStyle topRight = FFMStyles.Text(TextAnchor.UpperRight);
			GUI.Label(SRect.screen, "Ping: " + Network.GetLastPing(Network.connections[0]), topRight);
		}

		if (_selectedItem != null)
		{
			if (Vector3.Distance(GameData.activePlayer.GetCharacterTransform().position, _selectedItem.GetTransform().position) < 5.0f)
			{ // Draw the menu
				_droppedItem.enabled = true;
			}
			else
			{
				_selectedItem = null;
				_droppedItem.enabled = false;
			}
		}
		else
		{
			_droppedItem.enabled = false;
		}

		// NOTE: This system and layout is temporary! (maybe, I don't even know anymore)
		if (_state == HUDState.SkillTree)
		{
			SkillTreeScript.DrawSkillTree();
		}
		else if (_state == HUDState.Wiki)
		{
			WikiManager.DrawWiki();
		}
		else if (GameData.gamePaused)
		{
			DrawPauseMenu(SRect.Make(0.25f * w, 0.25f * h, 0.5f * w, 0.5f * h, "pause_window"));
		}
		else
		{
			DrawExitButton();

			DrawWikiButton();

			_dataView.SetObject(null); // TEMPORARY

			if (hudRoot.enabled)
			{
				hudRoot.Render();
			}
		}
	}

	public static void SetDataViewObject(WikiEntry obj)
	{
		_dataView.SetObject(obj);
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

	public static HUDState GetState()
	{
		return _state;
	}

	public static void OpenMenu(HUDMenu menu)
	{ // THIS IS TEMPORARY (maybe)
		_inventory.enabled = (menu == HUDMenu.Inventory);
		_skills.enabled = (menu == HUDMenu.QuickSkill);
		_spells.enabled = (menu == HUDMenu.SpellSlot);
	}

	public static void SetSlot(SpellSlot slot)
	{ // This is getting stupid
		_activeSlot = slot;
	}

	public static SpellSlot GetSlot()
	{ // I really hope this doesn't stay that way
		return _activeSlot;
	}

	public static void SetSelectedItem(DroppedItemScript item)
	{
		_selectedItem = item;
		if (item != null)
		{
			Vector3 camPos = GameData.activePlayer.GetCameraScript().GetCamera().WorldToScreenPoint(item.GetTransform().position);
			_droppedItem.SetPos(camPos.x - _droppedItem.GetFrame().width / 2, Screen.height - camPos.y - _droppedItem.GetFrame().height / 2);
		}
	}

	public static DroppedItemScript GetSelectedItem()
	{
		return _selectedItem;
	}

	private static void DrawPauseMenu(Rect rect)
	{ // Game is paused for whatever reason
		float w = rect.width;
		float h = rect.height;
		Rect localRect = SRect.Make(0.0f, 0.0f, w, h, "pause_menu");

		GUI.BeginGroup(rect);

		//GUIStyle bottom = FFMStyles.Text(TextAnchor.LowerCenter, bottomPadding: 2);

		// Background
		GUI.Box(localRect, GUIContent.none);

		string message = "???";

		if (GameData.gamePaused)
		{
			if (GameData.pauseMessage == PauseMessage.PLAYER1_VICTORY)
			{
				message = "Le joueur 1 a gagné!";
			}
			else if (GameData.pauseMessage == PauseMessage.PLAYER2_VICTORY)
			{
				message = "Le joueur 2 a gagné!";
			}
			else if (GameData.pauseMessage == PauseMessage.CLIENT_RECONNECT)
			{
				if (GameData.networkError != NetworkConnectionError.NoError)
				{
					message = "La connexion au serveur a été perdue.\n\nReconnexion en cours...\n\nErreur: " + GameData.networkError.ToString();
				}
				else
				{
					message = "La connexion au serveur a été perdue.\n\nReconnexion en cours...";
				}
			}
			else if (GameData.pauseMessage == PauseMessage.CLIENT_DROP)
			{
				message = "Le serveur s'est déconnecté.";
			}
			else if (GameData.pauseMessage == PauseMessage.SERVER_WAITING)
			{
				message = "En attente d'un autre joueur...";
			}
			else if (GameData.pauseMessage == PauseMessage.LOST_CLIENT)
			{
				message = "En attente de la reconnexion d'un ou plusieurs joueur(s)...";
			}
			else if (GameData.pauseMessage == PauseMessage.INCORRECT_GAMEMODE)
			{
				message = "Le mode choisi ne correspond pas à celui du serveur";
			}
			else if (GameData.pauseMessage == PauseMessage.INCORRECT_SECURITY)
			{
				message = "Le niveau de sécurité choisi ne correspond pas à celui du serveur";
			}
			else if (GameData.pauseMessage == PauseMessage.LOADING)
			{
				message = "Les autres joueurs chargent la partie...";
			}
		}

		GUI.Box(localRect, message, FFMStyles.centeredText_wrapped);

		if (GUI.Button(SRect.Make(0.4f * w, 0.8f * h, 0.2f * w, 0.2f * h, "pause_exit"), "Quitter"))
		{
			BackToMainMenu();
		}

		GUI.EndGroup();
	}

	private static void DrawExitButton()
	{
		if (GUI.Button(SRect.Make(0, 0, 100, 20, "exit"), "Quitter"))
		{
			isLeaving = !isLeaving;
		}

		if (isLeaving)
		{
			if (GUI.Button(SRect.Make(0, 20, 100, 20, "exit_confirm"), "Confirmer"))
			{
				isLeaving = false;
				BackToMainMenu();
			}
			if (GUI.Button(SRect.Make(100, 20, 100, 20, "exit_cancel"), "Annuler"))
			{
				isLeaving = false;
			}
		}
	}

	private static void DrawWikiButton()
	{
		if (GUI.Button(SRect.Make(Screen.width / 2 - 30, 0, 60, 20, "help"), "Aide"))
		{
			SetState(HUDState.Wiki);
		}
	}
}
