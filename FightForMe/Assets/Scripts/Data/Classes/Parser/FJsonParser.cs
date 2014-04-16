using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FJsonParser {
	
	private static FJsonParser me = null;
	
	static List<Clazz> results = null;
	
	static Dictionary<string, string> fields;
	static string value;
	static string clazz;
	static string key;
	static bool clazzFound;
	static bool skipingMode;
	
	static string pattern ;
	static int index;
	static StreamReader config;
	
	public static FJsonParser Instance(){
		if (me == null){
			me = new FJsonParser();
		}
		return me;
	}
	
	private FJsonParser (){
		reset();
	}
	
	public List<Clazz> getResults(){
		return results;
	}
	
	private void reset(){
		results = new List<Clazz>();
		fields = new Dictionary<string, string>();
		value = null;
		clazz = null;
		key = null;
		clazzFound = false;
		skipingMode = false;
		pattern = null;
		index = 0;
		config = null;
	}
	
	public void parseFile(string path){
		reset();
		
		config = File.OpenText (path);
		int curr = config.Read ();
		Debug.Log ("Start parsing ...");
		while (-1 != curr) {
			parse (curr, config.Peek());
			curr = config.Read ();
		}
		Debug.Log ("End of parsing ...");
		config.Close ();
	}
	
	public void parseString(string str){
		reset();
		pattern = str;
		int curr = pattern.Length > 0 ? pattern[0] : -1;
		Debug.Log ("Start parsing ...");
		while (curr != -1) {
			parse(curr, index+1 < pattern.Length ? pattern[index+1] : -1);
			curr = nextIndex();
		}
		Debug.Log ("End of parsing ...");
	}
	
	private void parse(int curr, int next){
		if (curr == '#'&& (	next != -1 || next != '\\')){
			skipingMode = !skipingMode;
			return;
		}else
		if (skipingMode){
			return;
		}
		
		if (curr == '\t' || curr == '\n' || curr == '\r' || curr == ' '){
			return;
		}
		
		if (curr == '"'){
			parseValue();
			if(null == value){
				Debug.LogWarning("Value is not set for key : '" + key + "'");
				return;
			}
			
			if(key != null){
				fields.Add(key,value);
				key = "";
				value = "";
			}else{
				Debug.LogError("Missing key for value : "+value);
				return;
			}
			return;
		}
		
		if (curr == ':'){
			if(!clazzFound){
				clazz = key;
				if(clazz  == null){
					Debug.LogError("Missing clazz");
					return;
				}
				key = "";
			}
			return;
		}
		
		if (curr == '{'){
			if(!clazzFound){
				clazzFound = true;
			} else {
				parseMap();
				if(key != null){
					fields.Add(key,value);
					key = "";
					value = "";
				}else{
					Debug.LogError("Missing key for value : "+value);
					return;
				}
			}
			return;
		}
		
		if(curr == '}'){
			if(clazzFound){
				results.Add(new Clazz(clazz, fields));
				clazz = "";
				clazzFound = false;
				key = "";
				value = "";
				fields.Clear();
			} else {
				Debug.LogError("Missing clazz and starting '}'");
				return;
			}
			return;
		}
		
		
		key+=(char)curr;
	}
	
	private void parseValue(){
		int curr = 0;
		
		if (config != null) {
			curr = config.Read ();
		} else if (pattern != null) {
			curr = nextIndex();
		} else {
			Debug.LogError ("parseValue > Error : nothing to parse");
			return;
		}
		
		//string value = "";
		while (curr != '"') {
			if(curr == -1){
				Debug.LogError("Error in parsing, missing '\"' ending");
				break;
			}
			value+=(char)curr;
			
			if(config != null){
				curr = config.Read();
			} else if (pattern != null){
				curr = nextIndex();;
			} else {
				Debug.LogError("parseValue > Error : nothing to parse");
				return;
			}
		}
	}
	
	private void parseMap(){
		int curr = 0,
		toEnd = 1;
		
		if (config != null) {
			curr = config.Read ();
		} else if (pattern != null) {
			curr = nextIndex();
		} else {
			Debug.LogError ("parseMap > Error : nothing to parse");
			return;
		}
		
		//string value = "";
		while(toEnd > 0){
			
			if(curr == -1){
				Debug.LogError("Error in sub Map parsing, missing '}' ending");
				break;
			}
			
			if(curr == '{'){
				toEnd++;
			}
			
			if(curr == '}'){
				toEnd--;
			}
			
			if(toEnd > 0) // tricky tricky
				value += (char)curr;
			
			if(config != null){
				curr = config.Read();
			} else if (pattern != null){
				curr = nextIndex();
			} else
				Debug.LogError("parseMap > Error : nothing to parse");
		}
		
	}
	
	private int nextIndex(){
		if (null == pattern || index + 1 < 0) {
			Debug.LogWarning("No string to parse");
			return -1;
		}
		if (index + 1 < pattern.Length) {
			index ++;
			return pattern [index];
		} 
		
		return -1;
	}
	
}
