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
			GameData.pauseMessage = PauseMessage.PLAYER1_VICTORY;
		}
		else if (this.gameObject.layer == LayerMask.NameToLayer("Team2Objective"))
		{ // Player 2 won
			GameData.pauseMessage = PauseMessage.PLAYER2_VICTORY;
		}

		GameData.gamePaused = true;
	}
}
