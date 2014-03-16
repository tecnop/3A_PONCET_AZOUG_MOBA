using UnityEngine;
using System.Collections;

public class PlayerHUDScript : MonoBehaviour
{
	//private CharacterManager _manager;

	//private CharacterStatsScript _stats;

	//private PlayerMiscDataScript _misc;

	public void Initialize(CharacterManager manager)
	{
		//this._manager = manager;
		//this._stats = manager.GetStatsScript();
		//this._misc = ((PlayerMiscDataScript)manager.GetMiscDataScript());
		HUDRenderer.Initialize();
	}

	void OnGUI()
	{
		HUDRenderer.Render();
	}
}
