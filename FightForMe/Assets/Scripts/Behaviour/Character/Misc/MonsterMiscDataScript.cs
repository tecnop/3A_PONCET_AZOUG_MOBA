using UnityEngine;
using System.Collections;

public class MonsterMiscDataScript : CharacterMiscDataScript
{
	uint pendingSetup = 0;

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
		if (pendingSetup != 0)
		{
			SetUpFromMonster(pendingSetup);
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			int test = (int)pendingSetup;
			stream.Serialize(ref test);
		}
		else
		{
			int test = 0;
			stream.Serialize(ref test);
			pendingSetup = (uint)test;
		}
	}

	public void SetUpFromMonster(uint monsterID)
	{
		if (_manager == null)
		{ // Delay it
			pendingSetup = monsterID;
			return;
		}

		if (GameData.isOnline)
		{
			_networkView.RPC("DoSetUp", RPCMode.All, (int)monsterID);
		}
		else
		{
			DoSetUp((int)monsterID);
		}
	}

	[RPC]
	private void DoSetUp(int monsterID)
	{
		Monster monster = DataTables.GetMonster((uint)monsterID);

		if (monster == null)
		{ // Hmmm...
			Debug.LogWarning("Tried to setup unknown monster " + monsterID + " on entity " + _manager.name);
			return;
		}

		if (GameData.isServer)
		{
			_manager.MakeLocal();
		}

		// Set him the data we got from the data table
		_manager.name = monster.GetName();
		((NPCAIScript)_manager.GetInputScript()).SetBehaviour(monster.GetBehaviour());
		_manager.GetInventoryScript().SetItems(monster.GetItems());

		_manager.GetGraphicsLoader().LoadModel(monster.GetModel());
		//_manager.GetCharacterTransform().localScale *= monster.GetScale();
	}
}
