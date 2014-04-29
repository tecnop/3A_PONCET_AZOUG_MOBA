using UnityEngine;
using System.Collections;

public class TileBuilderScript : MonoBehaviour
{
	[SerializeField]
	private Transform terrain;

	[SerializeField]
	private float tileSpacing = 3.0f;

	void Start()
	{ // Build the tile map

		if (!GameData.wentThroughMenu)
		{ // Just don't bother, we're debugging and this system no longer needs testing
			Destroy(this.gameObject);
			return;
		}

		float terrainSizeX = 10.0f * terrain.localScale.x;
		float terrainSizeZ = 10.0f * terrain.localScale.z;

		int sizeX = Mathf.FloorToInt(terrainSizeX / tileSpacing);
		int sizeZ = Mathf.FloorToInt(terrainSizeZ / tileSpacing);

		Vector3 startPos = new Vector3(-0.5f * terrainSizeX, 0, -0.5f * terrainSizeZ);

		TileManager.GenerateTiles(startPos, sizeX, sizeZ, tileSpacing);

		TileManager.BuildNeighbours();

		// Don't need us anymore once we're done
		Destroy(this.gameObject);
	}

	void OnGUI()
	{
		GUI.Label(new Rect(0.0f, 0.0f, Screen.width, Screen.height), "Loading : " + TileManager.generationProgress * 100 + "%", FFMStyles.centeredText);
	}
}
