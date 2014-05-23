using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileBuilderScript : MonoBehaviour
{
	[SerializeField]
	private Transform terrain;			// Ground entity

	[SerializeField]
	private float tileSpacing = 3.0f;	// Distance between each tile if we are to generate grid

	[SerializeField]
	private MapGrid grid;				// Pre-generated grid

	void Start()
	{ // Build the tile map
		if (grid != null)
		{ // Tiles have been generated already
			LoadGrid();

			TileManager.BakeNeighbours();
		}
		else
		{ // Do it right now
			if (!GameData.wentThroughMenu)
			{ // Just don't bother, we're debugging and this system no longer needs testing; create a global tile so everyone can see each other
				float terrainSizeX = 10.0f * terrain.localScale.x;
				float terrainSizeZ = 10.0f * terrain.localScale.z;
				Vector3 startPos = Vector3.zero;
				TileManager.GenerateTiles(startPos, 1, 1, 2.0f * Mathf.Max(terrainSizeX, terrainSizeZ));
				Destroy(this.gameObject);
				return;
			}

			Generate();

			TileManager.BakeNeighbours();
		}

		// Don't need us anymore once we're done
		Destroy(this.gameObject);
	}

	public void SetGrid(MapGrid tiles)
	{
		this.grid = tiles;
	}

	public bool HasGrid()
	{
		return this.grid != null;
	}

	public void LoadGrid()
	{
		TileManager.SetGrid(this.grid);
	}

	public void Generate(bool buildNeighbours = true)
	{
		float terrainSizeX = 10.0f * terrain.localScale.x;
		float terrainSizeZ = 10.0f * terrain.localScale.z;

		int sizeX = Mathf.CeilToInt(terrainSizeX / tileSpacing);
		int sizeZ = Mathf.CeilToInt(terrainSizeZ / tileSpacing);

		Vector3 startPos = new Vector3(-0.5f * terrainSizeX, 0, -0.5f * terrainSizeZ);

		TileManager.GenerateTiles(startPos, sizeX, sizeZ, tileSpacing);

		if (buildNeighbours)
		{
			TileManager.BuildNeighbours();
		}
	}

	/*void OnGUI()
	{
		GUI.Label(new Rect(0.0f, 0.0f, Screen.width, Screen.height), "Loading : " + TileManager.generationProgress * 100 + "%", FFMStyles.centeredText);
	}*/
}
