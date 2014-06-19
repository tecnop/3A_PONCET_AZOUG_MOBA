using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialZoneScript : MonoBehaviour
{
	private static int currentProgress = 0; // Only zones whose identifier is equal to this will be displayed

	[SerializeField]
	private int identifier;

	[SerializeField]
	private string message;

	private List<Collider> entities;

	void Start()
	{
		if (GameData.gameMode != GameMode.Tutorial)
		{
			Destroy(this.gameObject);
			return;
		}

		if (currentProgress != 0) currentProgress = 0;

		entities = new List<Collider>();

		// Replace all occurences of "\n" with actual line breaks
		message = message.Replace("\\n", "\n");
	}

	void OnTriggerEnter(Collider col)
	{
		if (entities.Contains(col))
		{
			return;
		}

		entities.Add(col);
	}

	void OnTriggerExit(Collider col)
	{
		if (!entities.Contains(col))
		{
			return;
		}

		entities.Remove(col);
	}

	void OnGUI()
	{
		if (HUDRenderer.GetState() == HUDState.Default)
		{
			if (entities.Count > 0 && currentProgress == this.identifier)
			{
				float w = 300.0f;
				float h = 150.0f;

				GUI.BeginGroup(new Rect(10.0f, 0.5f * Screen.height - h / 2, w, h));

				GUI.Box(new Rect(0.0f, 0.0f, w, h), GUIContent.none);
				GUI.Label(new Rect(0.0f, 0.0f, w, h - 20), message, FFMStyles.centeredText_wrapped);

				if (GUI.Button(new Rect(0.4f * w, h - 20, 0.2f * w, 20.0f), "OK"))
				{
					currentProgress++;
				}

				GUI.EndGroup();
			}
		}
	}
}
