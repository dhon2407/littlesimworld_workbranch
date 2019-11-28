using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : ScriptableObject
{
	public static List<Recipe> Recipes => instance.recipes;

	static RecipeManager _instance;
	static RecipeManager instance {
		get {
			if (_instance == null) { _instance = Resources.Load<RecipeManager>("Recipe Manager"); }
			return _instance;
		}
	}

	public List<Recipe> recipes;
}
