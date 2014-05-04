using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileBuilderScript : MonoBehaviour
{
	[SerializeField]
	private Transform terrain;

	[SerializeField]
	private float tileSpacing = 3.0f;

	[SerializeField]
	private List<List<MapTile>> tiles;

	void Start()
	{ // Build the tile map
		float terrainSizeX = 10.0f * terrain.localScale.x;
		float terrainSizeZ = 10.0f * terrain.localScale.z;

		int sizeX = Mathf.CeilToInt(terrainSizeX / tileSpacing);
		int sizeZ = Mathf.CeilToInt(terrainSizeZ / tileSpacing);

		Vector3 startPos = new Vector3(-0.5f * terrainSizeX, 0, -0.5f * terrainSizeZ);

		if (tiles != null && tiles.Count > 0)
		{ // Tiles have been generated already
			TileManager.SetTiles(tiles, startPos);
		}
		else
		{ // Do it right now
			if (!GameData.wentThroughMenu)
			{ // Just don't bother, we're debugging and this system no longer needs testing
				Destroy(this.gameObject);
				return;
			}

			TileManager.GenerateTiles(startPos, sizeX, sizeZ, tileSpacing);

			TileManager.BuildNeighbours();
		}

		// Don't need us anymore once we're done
		Destroy(this.gameObject);
	}

	public void SetTiles(List<List<MapTile>> tiles)
	{
		this.tiles = tiles;
	}

	void OnGUI()
	{
		GUI.Label(new Rect(0.0f, 0.0f, Screen.width, Screen.height), "Loading : " + TileManager.generationProgress * 100 + "%", FFMStyles.centeredText);
	}
}
