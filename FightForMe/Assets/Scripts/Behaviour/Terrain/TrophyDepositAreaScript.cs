using UnityEngine;
using System.Collections;

public class TrophyDepositAreaScript : MonoBehaviour
{
	// Rushed static variables for my AI
	private static TrophyDepositAreaScript _team1Area;
	public static TrophyDepositAreaScript team1Area
	{
		get
		{
			return _team1Area;
		}
	}

	private static TrophyDepositAreaScript _team2Area;
	public static TrophyDepositAreaScript team2Area
	{
		get
		{
			return _team2Area;
		}
	}

	[SerializeField]
	private NetworkView _networkView;

	[SerializeField]
	private Transform _transform;

	void Start()
	{
		if (GameData.gameMode != GameMode.RaceForGlory)
		{
			Destroy(this.gameObject);
		}

		if (this.gameObject.layer == LayerMask.NameToLayer("Team1Objective"))
		{
			_team1Area = this;
		}
		else if (this.gameObject.layer == LayerMask.NameToLayer("Team2Objective"))
		{
			_team2Area = this;
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
					_networkView.RPC("GameOver", RPCMode.AllBuffered);
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

	public Transform GetTransform()
	{
		return _transform;
	}
}
