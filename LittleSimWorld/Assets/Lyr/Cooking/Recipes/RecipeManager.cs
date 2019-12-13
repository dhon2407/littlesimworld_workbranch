using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : ScriptableObject
{
	private static RecipeManager _instance;
	public static List<Cooking.NewRecipe> Recipes => instance.recipes;
	
    public List<Cooking.NewRecipe> recipes;

	static RecipeManager instance {
		get {
			if (_instance == null) { _instance = Resources.Load<RecipeManager>("Recipe Manager"); }
			return _instance;
		}
	}
}
