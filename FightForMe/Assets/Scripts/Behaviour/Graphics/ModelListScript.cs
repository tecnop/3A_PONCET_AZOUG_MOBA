using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * ModelListScript.cs
 * 
 * Receives a list of models from loaded resources and allows data tables and entities to access them
 * 
 */

public class ModelListScript : MonoBehaviour
{
	[SerializeField]
	private GameObject[] models;

	void Start()
	{
		DataTables.LoadModels(models);
		Destroy(this.gameObject);
	}
}
