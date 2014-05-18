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
	Wiki		// Showing the wiki
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

		_state = HUDState.Default;
		_activeSlot = SpellSlot.NUM_SLOTS;

		hudRoot = new HUDContainer("HUD_root", new Rect(0, 0, w, h));

		new HUDBuffDisplay(new Rect(0.4f * w, 0.7f * h, 0.2f * w, 0.1f * h), hudRoot);
		new HUDBar(new Rect(0.0f, 0.8f * h, w, 0.2f * h), hudRoot);
		_inventory = new HUDInventory(new Rect(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h), hudRoot);
		_skills = new HUDQuickSkills(new Rect(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h), hudRoot);
		_spells = new HUDSpellWindow(new Rect(0.3f * w, 0.5f * h, 0.4f * w, 0.3f * h), hudRoot);
		_droppedItem = new HUDDroppedItemWindow(new Rect(0.3f * w, 0.5f * h, 0.4f * w, 0.3f * h), hudRoot);
		_dataView = new HUDDataView(new Rect(0.0f, 0.4f * h, 0.35f * w, 0.3f * h), hudRoot);
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
		if (_state == HUDState.Wiki)
		{
			WikiManager.DrawWiki();
		}
		else if (GameData.gamePaused)
		{
			DrawPauseMenu(new Rect(0.25f * w, 0.25f * h, 0.5f * w, 0.5f * h));
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
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

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
			else if (GameData.pauseMessage == PauseMessage.CLIENT_CONNECT)
			{
				if (GameData.networkError != NetworkConnectionError.NoError)
				{
					message = "Tentative de connexion au serveur...\n\nErreur: " + GameData.networkError.ToString();
				}
				else
				{
					message = "Tentative de connexion au serveur...";
				}
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
			else if (GameData.pauseMessage == PauseMessage.CLIENT_KICK)
			{ // Never gonna happen! Why am I even doing this
				message = "Vous avez été exclu(e) de la partie.";
			}
			else if (GameData.pauseMessage == PauseMessage.SERVER_INITIALIZING)
			{
				message = "Mise en place du serveur...";
			}
			else if (GameData.pauseMessage == PauseMessage.SERVER_WAITING)
			{
				message = "En attente d'un autre joueur...";
			}
			else if (GameData.pauseMessage == PauseMessage.SERVER_FAILURE)
			{
				message = "L'initialisation du serveur a échoué.\n\nSi ce message apparait pour plus de 5 secondes, vérifiez votre connexion à un réseau et relancez le jeu.";
			}
			else if (GameData.pauseMessage == PauseMessage.CLIENT_WAITING)
			{
				message = "En attente d'autres joueurs...";
			}
			else if (GameData.pauseMessage == PauseMessage.LOST_CLIENT)
			{
				message = "En attente de la reconnexion d'un ou plusieurs joueur(s)...";
			}
		}

		GUI.Box(localRect, message, FFMStyles.centeredText_wrapped);

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
			isLeaving = true;
		}

		if (isLeaving)
		{
			if (GUI.Button(new Rect(0, 20, 100, 20), "Confirmer"))
			{
				isLeaving = false;
				BackToMainMenu();
			}
			if (GUI.Button(new Rect(100, 20, 100, 20), "Annuler"))
			{
				isLeaving = false;
			}
		}
	}

	private static void DrawWikiButton()
	{
		if (GUI.Button(new Rect(Screen.width / 2 - 30, 0, 60, 20), "Aide"))
		{
			SetState(HUDState.Wiki);
		}
	}
}
