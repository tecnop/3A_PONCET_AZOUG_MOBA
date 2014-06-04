using UnityEngine;
using System.Collections;

public class MonsterMiscDataScript : CharacterMiscDataScript
{
	private uint monsterID;

	private uint pendingSetup = 0;
	private bool hax = false; // Terrible terrible

	public override void Initialize(CharacterManager manager)
	{
		_manager = manager;
		if (pendingSetup != 0)
		{
			if (hax)
			{
				DoSetUp((int)pendingSetup);
			}
			else
			{
				SetUpFromMonster(pendingSetup);
			}
		}
	}

	public uint GetMonsterID()
	{
		return this.monsterID;
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
			_networkView.RPC("DoSetUp", RPCMode.AllBuffered, (int)monsterID);
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

		this.monsterID = (uint)monsterID;

		if (monster == null)
		{ // Hmmm...
			Debug.LogWarning("Tried to setup unknown monster " + monsterID + " on entity " + _manager.name);
			return;
		}

		if (!_manager)
		{ // Ew.
			pendingSetup = (uint)monsterID;
			hax = true;
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
		foreach (uint buffID in monster.GetBuffs())
		{
			_manager.GetCombatScript().ReceiveBuff(_manager, buffID);
		}

		_manager.GetGraphicsLoader().LoadModel(monster.GetModel());

		if (monster.GetModel() != null)
		{
			_manager.GetAnimatorScript().transform.position += new Vector3(0.0f, monster.GetModel().GetScale() - 1.0f, 0.0f);
		}
	}
}
