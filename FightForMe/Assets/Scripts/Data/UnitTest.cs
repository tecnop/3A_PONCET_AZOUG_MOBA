using UnityEngine;
using System.Collections;

public class UnitTest : MonoBehaviour
{
	void Start()
	{ // Temporary entity used to initialize the data tables
		DataTables.updateTables();
		Destroy(this.gameObject);
	}
}
