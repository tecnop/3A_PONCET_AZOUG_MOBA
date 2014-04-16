using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Clazz {
	string name;
	Dictionary<string, string> fields;
	
	public Clazz(string name, Dictionary<string,string> fields){
		this.name = name;
		this.fields = new Dictionary<string, string> ();
		foreach (KeyValuePair<string, string> k in fields) {
			this.fields.Add(k.Key, k.Value);
		}
	}
	
	public string getName(){
		return this.name;
	}
	
	
	public Dictionary<string, string> getFields(){
		return this.fields;
	}
}