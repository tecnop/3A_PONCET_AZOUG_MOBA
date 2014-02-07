using UnityEngine;
using System.Collections;

public enum HUDMenu
{
	None,
	Inventory,
	Skill
}

public class PlayerHUDScript : MonoBehaviour
{
	private CharacterManager _manager;

	private CharacterStatsScript _stats;

	private PlayerMiscDataScript _misc;

	private bool advancedStats;

	private HUDMenu curMenu;

	public void _Initialize(CharacterManager manager)
	{
		this._manager = manager;
		this._stats = manager.GetStatsScript();
		this._misc = ((PlayerMiscDataScript)manager.GetMiscDataScript());
		advancedStats = false;
		curMenu = HUDMenu.None;
	}

	void OnGUI()
	{ // I was going to use groups but I feel like it would just make things even more complicated for now... this is just a test
		int w = Screen.width;
		int h = Screen.height;

		GUIStyle mid = new GUIStyle();
		mid.wordWrap = false;
		mid.alignment = TextAnchor.MiddleCenter;

		// TODO: Buffs

		{ // Lower bar
			GUI.Box(new Rect(0.0f, 0.8f * h, w, 0.2f * h), GUIContent.none);

			{ // Stats panel
				Rect statsRect = new Rect(0.025f * w, 0.825f * h, 0.2f * w, 0.15f * h);
				if (GUI.Button(statsRect, GUIContent.none))
				{
					advancedStats = !advancedStats;
				}

				if (!advancedStats)
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
				{ // Background
					GUIStyle right = new GUIStyle(mid);
					right.alignment = TextAnchor.MiddleRight;
					right.padding.right = 5;
					Rect barRect = new Rect(0.25f * w, 0.85f * h, 0.5f * w, 0.05f * h);
					GUI.Box(barRect, GUIContent.none);
					GUI.Label(barRect, Mathf.Ceil(curHealth) + " / " + Mathf.Ceil(maxHealth) + " HP", mid);
					GUI.Label(barRect, "+ " + _stats.GetHealthRegen() + "/s", right);
				}
				if (curHealth > 1)
				{ // Bar
					GUI.Box(new Rect(0.25f * w, 0.85f * h, (curHealth / maxHealth) * 0.5f * w, 0.05f * h), GUIContent.none);
				}
			}

			{ // Mana bar
				float curMana = _stats.GetMana();
				float maxMana = _stats.GetMaxMana();
				{ // Background
					GUIStyle right = new GUIStyle(mid);
					right.alignment = TextAnchor.MiddleRight;
					right.padding.right = 5;
					Rect barRect = new Rect(0.25f * w, 0.9f * h, 0.5f * w, 0.05f * h);
					GUI.Box(barRect, GUIContent.none);
					GUI.Label(barRect, Mathf.Ceil(curMana) + " / " + Mathf.Ceil(maxMana) + " MP", mid);
					GUI.Label(barRect, "+ " + _stats.GetManaRegen() + "/s", right);
				}
				if (curMana > 1)
				{ // Bar
					GUI.Box(new Rect(0.25f * w, 0.9f * h, (curMana / maxMana) * 0.5f * w, 0.05f * h), GUIContent.none);
				}
			}

			{ // Menu buttons
				if (GUI.Button(new Rect(0.775f * w, 0.825f * h, 0.2f * w, 0.075f * h), "Inventaire"))
				{
					curMenu = HUDMenu.Inventory;
				}

				if (GUI.Button(new Rect(0.775f * w, 0.9f * h, 0.2f * w, 0.075f * h), "Compétences"))
				{
					curMenu = HUDMenu.Skill;
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

			if (curMenu != HUDMenu.None)
			{ // Menus
				Rect menuRect = new Rect(0.75f * w, 0.2f * h, 0.25f * w, 0.6f * h);

				GUI.Box(menuRect, GUIContent.none);

				float menuW = 0.25f * w;
				float menuH = 0.6f * h;
				GUI.BeginGroup(menuRect);

				if (curMenu == HUDMenu.Inventory)
				{
					ArrayList objects = _manager.GetInventoryScript().GetItems();
					uint i=0;
					foreach (Item item in objects)
					{
						if (GUI.Button(new Rect(0.0f, 40.0f * i, menuW, 40.0f * (i+1)), item.getName()))
						{
							_manager.GetInventoryScript().DropItem(i);
						}
						i++;
					}
				}
				else if (curMenu == HUDMenu.Skill)
				{
					ArrayList objects = _misc.GetAvailableSkills();
					uint i=0;
					foreach (Skill skill in objects)
					{
						if (GUI.Button(new Rect(0.0f, 40.0f * i, menuW, 40.0f * (i + 1)), skill.GetName()))
						{
							_misc.LearnSkill(skill);
						}
						i++;
					}
				}

				if (GUI.Button(new Rect(0.25f * menuW, 0.9f * menuH, 0.5f * menuW, 0.1f * menuH), "Fermer"))
				{ // Exit button
					curMenu = HUDMenu.None;
				}

				GUI.EndGroup();
			}
		}
	}
}
