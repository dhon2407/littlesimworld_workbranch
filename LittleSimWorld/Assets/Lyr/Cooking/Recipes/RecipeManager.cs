//using System.Collections;
//using System.Collections.Generic;
//using InventorySystem;
//using UnityEngine;

//public class RecipeManager : ScriptableObject
//{
//	private static RecipeManager _instance;
	
//    public static List<Cooking.NewRecipe> Recipes => instance.recipes;
//    public List<Cooking.NewRecipe> recipes;

//    private Dictionary<ItemCode, List<Item>> recipeRequirements;


//    public static List<Item> GetItemRequirement(ItemCode recipe)
//    {
//        if (_instance.recipeRequirements.ContainsKey(recipe))
//            return _instance.recipeRequirements[recipe];
        
//        return null;
//    }

//    private static RecipeManager instance { get { if (_instance == null) Initialize(); return _instance; } }

//    private static void Initialize()
//    {
//        _instance = Resources.Load<RecipeManager>("Recipe Manager");
//        _instance.recipeRequirements = new Dictionary<ItemCode, List<Item>>();

//        foreach (var recipe in Recipes)
//        {
//            _instance.recipeRequirements[recipe.RecipeOutcome] = new List<Item>();
//            foreach (var itemRequired in recipe.itemsRequired)
//                _instance.recipeRequirements[recipe.RecipeOutcome].Add(Inventory.CreateItem(itemRequired));
//        }

//    }
//}
