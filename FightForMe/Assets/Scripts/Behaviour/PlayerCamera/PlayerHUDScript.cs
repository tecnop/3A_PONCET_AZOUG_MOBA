using UnityEngine;
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

public class PlayerHUDScript : MonoBehaviour
{
	private CharacterManager _manager;

	private CharacterStatsScript _stats;

	private PlayerMiscDataScript _misc;

	private bool _advancedStats;

	private HUDMenu _curMenu;

	private HUDState _state;

	public void Initialize(CharacterManager manager)
	{
		this._manager = manager;
		this._stats = manager.GetStatsScript();
		this._misc = ((PlayerMiscDataScript)manager.GetMiscDataScript());
		this._advancedStats = false;
		this._curMenu = HUDMenu.None;
		this._state = HUDState.Default;
	}

	private void BackToMainMenu()
	{
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

	public void SetState(HUDState state)
	{
		this._state = state;
	}

	private void DrawMinimap(Rect rect)
	{
		GUI.BeginGroup(rect);

		// TODO

		GUI.EndGroup();
	}

	private void DrawStats(Rect rect)
	{ // This needs some work, will do for now
		float w = rect.width;
		float h = rect.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(rect);

		if (GUI.Button(localRect, GUIContent.none))
		{
			_advancedStats = !_advancedStats;
		}

		if (!_advancedStats)
		{ // Main stats
			Stats stats = _stats.GetStats();
			string statsStr = "Endurance: " + stats.GetStrength() +
				"\nPuissance: " + stats.GetAgility() +
				"\nIntelligence: " + stats.GetIntelligence();

			GUI.Label(localRect, statsStr, FFMStyles.centeredText);
		}
		else
		{ // Misc stats
			string statsStr = "Dégâts: " + _stats.GetDamage() +
				"\n" + _stats.GetAttackRate() + " attaque(s)/s" +
				"\nVitesse: " + _stats.GetMovementSpeed() + " unités/s";

			GUI.Label(localRect, statsStr, FFMStyles.centeredText);
		}

		GUI.EndGroup();
	}

	private void DrawHealthBar(Rect rect)
	{
		float w = rect.width;
		float h = rect.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(rect);

		float curHealth = _stats.GetHealth();
		float maxHealth = _stats.GetMaxHealth();

		// Background
		GUI.Box(localRect, GUIContent.none);

		if (curHealth > 1)
		{ // Bar
			GUI.Box(new Rect(0.0f, 0.0f, (curHealth / maxHealth) * w , h), GUIContent.none);
		}
		
		// Text
		GUIStyle right = FFMStyles.Text(TextAnchor.MiddleRight, rightPadding: 5);
		GUI.Label(localRect, Mathf.Ceil(curHealth) + " / " + Mathf.Ceil(maxHealth) + " HP", FFMStyles.centeredText);
		GUI.Label(localRect, "+ " + _stats.GetHealthRegen() + "/s", right);

		GUI.EndGroup();
	}

	private void DrawManaBar(Rect rect)
	{
		float w = rect.width;
		float h = rect.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(rect);

		float curMana = _stats.GetMana();
		float maxMana = _stats.GetMaxMana();

		// Background
		GUI.Box(localRect, GUIContent.none);

		if (curMana > 1)
		{ // Bar
			GUI.Box(new Rect(0.0f, 0.0f, (curMana / maxMana) * w, h), GUIContent.none);
		}
		
		// Text
		GUIStyle right = FFMStyles.Text(TextAnchor.MiddleRight, rightPadding: 5);
		GUI.Label(localRect, Mathf.Ceil(curMana) + " / " + Mathf.Ceil(maxMana) + " MP", FFMStyles.centeredText);
		GUI.Label(localRect, "+ " + _stats.GetManaRegen() + "/s", right);

		GUI.EndGroup();
	}

	private void DrawXPBar(Rect rect)
	{
		float w = rect.width;
		float h = rect.height;
		Rect localRect = new Rect(0.0f, 0.0f, w, h);

		GUI.BeginGroup(rect);

		uint curXP = _misc.GetExperience();

		// Background
		GUI.Box(localRect, GUIContent.none);

		if (curXP > 0)
		{ // Bar
			GUI.Box(new Rect(0.0f, 0.0f, ((float)curXP / 100.0f) * w, h), GUIContent.none);
		}
		
		// Text
		GUI.Label(localRect, curXP + " / " + 100 + " XP", FFMStyles.centeredText);

		GUI.EndGroup();
	}

	private void DrawMenuButtons(Rect rect)
	{
		float w = rect.width;
		float h = rect.height;

		GUI.BeginGroup(rect);

		Rect inventoryRect = new Rect(0.0f, 0.0f, w, 0.5f * h);
		Rect skillsRect = new Rect(0.0f, 0.5f * h, w, 0.5f * h);

		if (GUI.Button(inventoryRect, "Inventaire"))
		{
			_curMenu = HUDMenu.Inventory;
		}

		if (GUI.Button(skillsRect, "Compétences"))
		{
			_curMenu = HUDMenu.Skill;
		}


		ArrayList sets = _manager.GetInventoryScript().GetCompletedSets();
		if (sets.Count > 0)
		{
			GUIStyle bottom = FFMStyles.Text(TextAnchor.LowerCenter, bottomPadding: 2);
			GUI.Label(inventoryRect, "Panoplie complète:" + ((ArmorSet)sets[0]).GetName(), bottom);
		}

		uint skillPoints = _misc.GetSkillPoints();
		if (skillPoints > 0)
		{
			GUIStyle bottom = FFMStyles.Text(TextAnchor.LowerCenter, bottomPadding: 2);
			GUI.Label(skillsRect, skillPoints + " point(s) à attribuer", bottom);
		}

		GUI.EndGroup();
	}

	private void DrawBuffs(Rect rect)
	{
		GUI.BeginGroup(rect);

		// TODO

		GUI.EndGroup();
	}

	private void DrawHUDBar(Rect rect)
	{
		float w = rect.width;
		float h = rect.height;

		GUI.Box(rect, GUIContent.none);

		GUI.BeginGroup(rect);

		DrawMinimap(new Rect());
		DrawStats(new Rect(0.025f * w, 0.1f * h, 0.2f * w, 0.8f * h));
		DrawHealthBar(new Rect(0.25f * w, 0.25f * h, 0.5f * w, 0.25f * h));
		DrawManaBar(new Rect(0.25f * w, 0.5f * h, 0.5f * w, 0.25f * h));
		DrawXPBar(new Rect(0.3f * w, 0.75f * h, 0.4f * w, 0.125f * h));
		DrawMenuButtons(new Rect(0.775f * w, 0.1f * h, 0.2f * w, 0.8f * h));

		GUI.EndGroup();
	}

	private void DrawActiveMenu(Rect rect)
	{ // TODO: Add a slider to support an unlimited amount of items/skills
		float w = rect.width;
		float h = rect.height;

		GUI.Box(rect, GUIContent.none);

		GUI.BeginGroup(rect);

		if (_curMenu == HUDMenu.Inventory)
		{
			ArrayList objects = _manager.GetInventoryScript().GetItems();
			uint i = 0;
			foreach (Item item in objects)
			{
				if (GUI.Button(new Rect(0.0f, 40.0f * i, w, 40.0f), item.GetName()))
				{
					_manager.GetInventoryScript().DropItem(i);
				}
				i++;
			}
		}
		else if (_curMenu == HUDMenu.Skill)
		{
			ArrayList objects = new ArrayList(_misc.GetAvailableSkills());
			uint i = 0;
			foreach (Skill skill in objects)
			{
				if (GUI.Button(new Rect(0.0f, 40.0f * i, w, 40.0f), skill.GetName()))
				{
					_misc.LearnSkill(skill);
				}
				i++;
			}
		}

		if (GUI.Button(new Rect(0.25f * w, 0.9f * h, 0.5f * w, 0.1f * h), "Fermer"))
		{ // Exit button
			_curMenu = HUDMenu.None;
		}

		GUI.EndGroup();
	}

	private void DrawPauseMenu(Rect rect)
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

	private void DrawExitButton()
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

	void OnGUI()
	{
		float w = Screen.width;
		float h = Screen.height;

		if (GameData.gameType == GameType.ListenServer && Network.connections.Length > 0)
		{ // TEMPORARY
			GUIStyle topRight = FFMStyles.Text(TextAnchor.UpperRight);
			GUI.Label(new Rect(0, 0, w, h), "Ping: " + Network.GetLastPing(Network.connections[0]), topRight);
		}

		if ((_state == HUDState.Default || _state == HUDState.Leaving) && !GameData.gamePaused)
		{
			DrawExitButton();

			DrawBuffs(new Rect());

			DrawHUDBar(new Rect(0.0f, 0.8f * h, w, 0.2f * h));

			if (_curMenu != HUDMenu.None)
			{ // Menus
				DrawActiveMenu(new Rect(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h));
			}
		}
		else
		{
			DrawPauseMenu(new Rect(0.25f * w, 0.25f * h, 0.5f * w, 0.5f * h));
		}
	}
}
