using UnityEngine;
using System.Collections;

public class GameModel
{ // Some sort of shortcut to access model prefabs, it doesn't actually store them

	private string modelPath;
	private float scale;
	// TODO: Particle effects?

	public GameModel(string modelPath,
		float scale = 1.0f)
	{
		this.modelPath = modelPath;
		this.scale = Mathf.Clamp(scale, 0.1f, 10.0f);
	}

	public string GetModelPath() { return this.modelPath; }
	public float GetScale() { return this.scale; }
	public GameObject GetModel() { return DataTables.GetModel(this.modelPath); }
}