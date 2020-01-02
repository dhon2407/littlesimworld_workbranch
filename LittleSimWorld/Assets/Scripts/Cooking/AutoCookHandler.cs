using System.Collections.Generic;
using InventorySystem;
using PlayerStats;
using UnityEngine;

using static Cooking.ManualCookHandler;

namespace Cooking.Recipe
{
    public class AutoCookHandler
    {
        private readonly ItemCode defaultItemToCook;

        public AutoCookHandler(ItemCode defaultItemToCook)
        {
            this.defaultItemToCook = defaultItemToCook;
        }

        public ItemCode GetItem()
        {
            var lastCookItemCode = CookingEntity.LastCookedItem;
            return RecipeManager.HaveEnoughIngredients(lastCookItemCode, CookingHandler.AvailableIngredients) ?
                lastCookItemCode : GetCookableRecipe();
        }


        private ItemCode GetCookableRecipe()
        {
            var cookableRecipes = new List<ItemCode>();
            foreach (var recipe in RecipeManager.Recipes)
            {
                if (RecipeManager.HaveEnoughIngredients(recipe, CookingHandler.AvailableIngredients) &&
                    SlotRequiredLevel(recipe.itemsRequired.Count) <= Stats.SkillLevel(Skill.Type.Cooking))
                    cookableRecipes.Add(recipe.RecipeOutcome);
            }

            if (cookableRecipes.Count > 0)
                return cookableRecipes[Random.Range(0, cookableRecipes.Count - 1)];
            
            return defaultItemToCook;
        }
    }
}