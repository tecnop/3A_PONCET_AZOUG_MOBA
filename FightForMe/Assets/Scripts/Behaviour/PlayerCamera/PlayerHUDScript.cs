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
		// What about servers?

		Application.LoadLevel(0);
	}

	public void SetState(HUDState state)
	{
		this._state = state;
	}

	void OnGUI()
	{ // I was going to use groups but I feel like it would just make things even more complicated for now... this is just a test
		int w = Screen.width;
		int h = Screen.height;

		GUIStyle mid = new GUIStyle();
		mid.wordWrap = false;
		mid.alignment = TextAnchor.MiddleCenter;
		mid.normal.textColor = Color.white;

		if ((_state == HUDState.Default || _state == HUDState.Leaving) && !GameData.gamePaused)
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

			// TODO: Buffs

			{ // Lower bar
				GUI.Box(new Rect(0.0f, 0.8f * h, w, 0.2f * h), GUIContent.none);

				{ // Stats panel
					Rect statsRect = new Rect(0.025f * w, 0.825f * h, 0.2f * w, 0.15f * h);
					if (GUI.Button(statsRect, GUIContent.none))
					{
						_advancedStats = !_advancedStats;
					}

					if (!_advancedStats)
					{ // Main stats
						Stats stats = _stats.GetStats();
						string statsStr = "Endurance: " + stats.GetStrength() + "\nPuissance: " + stats.GetAgility() + "\nIntelligence: " + stats.GetIntelligence();

						GUI.Label(statsRect, statsStr, mid);
					}
					else
					{ // Misc stats
						string statsStr = "Dégâts: " + _stats.GetDamage() +
							"\n" + _stats.GetAttackRate() + " attaque(s)/s" +
							"\nVitesse: " + _stats.GetMovementSpeed() + " unités/s";

						GUI.Label(statsRect, statsStr, mid);
					}
				}

				{ // Health bar
					float curHealth = _stats.GetHealth();
					float maxHealth = _stats.GetMaxHealth();
					Rect barRect = new Rect(0.25f * w, 0.85f * h, 0.5f * w, 0.05f * h);
					{ // Background
						GUI.Box(barRect, GUIContent.none);
					}
					if (curHealth > 1)
					{ // Bar
						GUI.Box(new Rect(0.25f * w, 0.85f * h, (curHealth / maxHealth) * 0.5f * w, 0.05f * h), GUIContent.none);
					}
					{ // Text
						GUIStyle right = new GUIStyle(mid);
						right.alignment = TextAnchor.MiddleRight;
						right.padding.right = 5;
						GUI.Label(barRect, Mathf.Ceil(curHealth) + " / " + Mathf.Ceil(maxHealth) + " HP", mid);
						GUI.Label(barRect, "+ " + _stats.GetHealthRegen() + "/s", right);
					}
				}

				{ // Mana bar
					float curMana = _stats.GetMana();
					float maxMana = _stats.GetMaxMana();
					Rect barRect = new Rect(0.25f * w, 0.9f * h, 0.5f * w, 0.05f * h);
					{ // Background
						GUI.Box(barRect, GUIContent.none);
					}
					if (curMana > 1)
					{ // Bar
						GUI.Box(new Rect(0.25f * w, 0.9f * h, (curMana / maxMana) * 0.5f * w, 0.05f * h), GUIContent.none);
					}
					{ // Text
						GUIStyle right = new GUIStyle(mid);
						right.alignment = TextAnchor.MiddleRight;
						right.padding.right = 5;
						GUI.Label(barRect, Mathf.Ceil(curMana) + " / " + Mathf.Ceil(maxMana) + " MP", mid);
						GUI.Label(barRect, "+ " + _stats.GetManaRegen() + "/s", right);
					}
				}

				{ // XP bar
					uint curXP = _misc.GetExperience();
					Rect barRect = new Rect(0.3f * w, 0.95f * h, 0.4f * w, 0.025f * h);
					{ // Background
						GUI.Box(barRect, GUIContent.none);
					}
					if (curXP > 0)
					{ // Bar
						GUI.Box(new Rect(0.3f * w, 0.95f * h, ((float)curXP / 100.0f) * 0.4f * w, 0.025f * h), GUIContent.none);
					}
					{ // Text
						GUI.Label(barRect, curXP + " / " + 100 + " XP", mid);
					}
				}

				{ // Menu buttons
					if (GUI.Button(new Rect(0.775f * w, 0.825f * h, 0.2f * w, 0.075f * h), "Inventaire"))
					{
						_curMenu = HUDMenu.Inventory;
					}

					if (GUI.Button(new Rect(0.775f * w, 0.9f * h, 0.2f * w, 0.075f * h), "Compétences"))
					{
						_curMenu = HUDMenu.Skill;
					}

					uint skillPoints = _misc.GetSkillPoints();
					if (skillPoints > 0)
					{
						GUIStyle bottom = new GUIStyle(mid);
						bottom.alignment = TextAnchor.LowerCenter;
						bottom.padding.bottom = 2;
						GUI.Label(new Rect(0.775f * w, 0.9f * h, 0.2f * w, 0.075f * h), skillPoints + " point(s) à attribuer", bottom);
					}
				}

				if (_curMenu != HUDMenu.None)
				{ // Menus
					Rect menuRect = new Rect(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h);

					GUI.Box(menuRect, GUIContent.none);

					float menuW = 0.25f * w;
					float menuH = 0.6f * h;
					GUI.BeginGroup(menuRect);

					if (_curMenu == HUDMenu.Inventory)
					{
						ArrayList objects = _manager.GetInventoryScript().GetItems();
						uint i = 0;
						foreach (Item item in objects)
						{
							if (GUI.Button(new Rect(0.0f, 40.0f * i, menuW, 40.0f), item.GetName()))
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
							if (GUI.Button(new Rect(0.0f, 40.0f * i, menuW, 40.0f), skill.GetName()))
							{
								_misc.LearnSkill(skill);
							}
							i++;
						}
					}

					if (GUI.Button(new Rect(0.25f * menuW, 0.9f * menuH, 0.5f * menuW, 0.1f * menuH), "Fermer"))
					{ // Exit button
						_curMenu = HUDMenu.None;
					}

					GUI.EndGroup();
				}
			}
		}
		else
		{ // Game is paused for whatever reason
			Rect box = new Rect(0.25f * w, 0.25f * h, 0.5f * w, 0.5f * h);

			GUIStyle bottom = new GUIStyle(mid);
			bottom.alignment = TextAnchor.LowerCenter;
			bottom.padding.bottom = 2;

			GUI.Box(box, GUIContent.none);

			if (_state == HUDState.Won)
			{
				GUI.Box(box, "Le joueur 1 a gagné!", mid);
			}
			else if (_state == HUDState.Lost)
			{
				GUI.Box(box, "Le joueur 2 a gagné!", mid);
			}
			else if (GameData.gamePaused)
			{
				if (GameData.networkError != NetworkConnectionError.NoError)
				{
					GUI.Box(box, "En attente de connexion...\n\nErreur: " + GameData.networkError.ToString(), mid);
				}
				else
				{
					GUI.Box(box, "En attente de connexion...", mid);
				}
			}

			if (GUI.Button(new Rect(0.4f * w, 0.65f * h, 0.2f * w, 0.1f * h), "Quitter"))
			{
				BackToMainMenu();
			}
		}
	}
}
