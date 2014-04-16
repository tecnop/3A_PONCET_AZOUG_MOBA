using UnityEngine;
using System.Collections;

public class TrophyDepositAreaScript : MonoBehaviour
{
	[SerializeField]
	private NetworkView _networkView;

	void Start()
	{
		if (GameData.gameMode != GameMode.RaceForGlory)
		{
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		CharacterPhysicsScript phys = other.GetComponent<CharacterPhysicsScript>();
		if (phys != null)
		{
			if (phys.GetManager().GetStatsScript().HasSpecialEffect(MiscEffect.CARRYING_TROPHY))
			{
				if (GameData.isOnline)
				{
					_networkView.RPC("GameOver", RPCMode.All);
				}
				else
				{
					GameOver();
				}
			}
		}
	}

	[RPC]
	private void GameOver()
	{ // Copy pasted from LordSpawnerScript for now
		if (this.gameObject.layer == LayerMask.NameToLayer("Team1Objective"))
		{ // Player 1 won
			Debug.Log("Player 1 won!");
			//GameData.activePlayer.GetCameraScript().GetHUDScript().SetState(HUDState.Won);
			HUDRenderer.SetState(HUDState.Won);
		}
		else if (this.gameObject.layer == LayerMask.NameToLayer("Team2Objective"))
		{ // Player 2 won
			Debug.Log("Player 2 won!");
			//GameData.activePlayer.GetCameraScript().GetHUDScript().SetState(HUDState.Lost);
			HUDRenderer.SetState(HUDState.Lost);
		}

		GameData.gamePaused = true;
	}
}
