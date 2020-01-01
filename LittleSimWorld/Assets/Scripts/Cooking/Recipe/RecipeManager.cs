﻿using System;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

namespace Cooking.Recipe
{
    [CreateAssetMenu(fileName = "RecipeManager", menuName = "Cooking/Recipe Manager")]
    public class RecipeManager : ScriptableObject
    {
        private static RecipeManager instance;

        public static List<NewRecipe> Recipes => Instance.recipes;
        public List<NewRecipe> recipes;

        private Dictionary<ItemCode, List<Item>> recipeRequirements;

        private static RecipeManager Instance
        {
            get
            {
                if (instance == null) Initialize();
                return instance;
            }
        }

        public static List<Item> GetItemRequirement(ItemCode recipe)
        {
            if (instance.recipeRequirements.ContainsKey(recipe))
                return instance.recipeRequirements[recipe];

            return null;
        }

        public static bool HaveEnoughIngredients(NewRecipe recipe, List<ItemList.ItemInfo> availableIngredients)
        {
            var requirements = new Dictionary<ItemCode, int>();
            foreach (var requiredItem in GetItemRequirement(recipe.RecipeOutcome))
            {
                if (requirements.ContainsKey(requiredItem.Code))
                    requirements[requiredItem.Code]++;
                else
                    requirements[requiredItem.Code] = 1;
            }

            foreach (var requirement in requirements)
            {
                if (availableIngredients.Exists(ing => ing.itemCode == requirement.Key) &&
                    availableIngredients.Find(ing => ing.itemCode == requirement.Key).count >= requirement.Value)
                    continue;

                return false;
            }

            return true;
        }

        public static bool HaveEnoughIngredients(ItemCode itemCode, List<ItemList.ItemInfo> availableIngredients)
        {
            var recipe = GetRecipe(itemCode);
            if (recipe != null)
                return HaveEnoughIngredients(recipe, availableIngredients);

            Debug.LogWarning($"No recipe available for item {itemCode}.");
            return false;

        }

        private static NewRecipe GetRecipe(ItemCode itemCode)
        {
            foreach (var newRecipe in Recipes)
                if (newRecipe.RecipeOutcome == itemCode)
                    return newRecipe;
            
            return null;
        }

        private static void Initialize()
        {
            instance = Resources.Load<RecipeManager>("Cooking/RecipeManager");
            instance.recipeRequirements = new Dictionary<ItemCode, List<Item>>();

            foreach (var recipe in Recipes)
            {
                instance.recipeRequirements[recipe.RecipeOutcome] = new List<Item>();
                foreach (var itemRequired in recipe.itemsRequired)
                    instance.recipeRequirements[recipe.RecipeOutcome].Add(Inventory.CreateItem(itemRequired));
            }

        }

    }
}