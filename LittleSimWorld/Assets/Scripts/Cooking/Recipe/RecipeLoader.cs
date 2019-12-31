using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using InventorySystem;

namespace Cooking.Recipe
{
    public class RecipeLoader : MonoBehaviour
    {
        [SerializeField] private GameObject recipeSlot;
        [SerializeField] private Transform recipeContainer;
        [SerializeField] private CookList cookingList;
        [SerializeField] private CookingHandler handler;

        private List<RecipeSlot> slots;

        private void Awake()
        {
            slots = new List<RecipeSlot>();
        }

        public void FetchRecipes()
        {
            ClearItems();

            var recipes = RecipeManager.Recipes;
            foreach (var recipe in recipes)
            {
                slots.Add(Instantiate(recipeSlot, recipeContainer).GetComponent<RecipeSlot>());
                slots[slots.Count - 1].SetRecipe(recipe).CheckRequirement();
            }

            UpdateSlotsAction();
        }

        public void RefreshRecipeRequirements()
        {
            foreach (var slot in slots)
                slot.CheckRequirement();
        }

        private void UpdateSlotsAction()
        {
            foreach (var slot in slots)
                slot.SetSelectAction(() =>
                {
                    cookingList.AddItem(slot.Item);
                    TakeRequiredIngredients(slot.Item);
                    RefreshRecipeRequirements();
                });
        }

        private void TakeRequiredIngredients(Item item)
        {
            var requiredItems = RecipeManager.GetItemRequirement(item.Code);
            foreach (var requiredItem in requiredItems)
                handler.TakeIngredient(requiredItem);
        }

        private void ClearItems()
        {
            slots.Clear();
            for (var i = 0; i < recipeContainer.childCount; i++)
                Destroy(recipeContainer.GetChild(i).gameObject);
        }
    }
}