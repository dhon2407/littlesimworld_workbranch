using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using InventorySystem;
using UnityEditor;
using UnityEngine.Events;

namespace Cooking.Recipe
{
    public class CookList : MonoBehaviour
    {
        [SerializeField] private GameObject recipeSlot;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private CookingHandler handler;
        [SerializeField] private RecipeLoader recipeLoader;

        private List<CookSlot> cookingSlots;
        private void Awake()
        {
            cookingSlots = new List<CookSlot>();
        }

        private void Start()
        {
            ClearList();
        }

        public void AddItem(Item slotItem)
        {
            if (cookingSlots.Exists(cookingSlot => cookingSlot.Item.Code == slotItem.Code))
                cookingSlots.Find(cookingSlot => cookingSlot.Item.Code == slotItem.Code).AddQuantity();
            else
            {
                var cookSlot = Instantiate(recipeSlot, itemContainer).GetComponent<CookSlot>()?.
                    SetItem(slotItem.Code);

                cookingSlots.Add(cookSlot);
                
                cookSlot?.
                    SetClearAction(() => cookingSlots.Remove(cookSlot)).
                    SetClickAction(() => ReturnItem(cookSlot.Item));
            }
        }

        public void ReturnItem(Item item)
        {
            var requiredItems = RecipeManager.GetItemRequirement(item.Code);
            foreach (var requiredItem in requiredItems)
                handler.ReturnIngredient(requiredItem);

            recipeLoader.RefreshRecipeRequirements();
        }

        private void ClearList()
        {
            cookingSlots.Clear();
            for (var i = 0; i < itemContainer.childCount; i++)
                Destroy(itemContainer.GetChild(i).gameObject);
        }
    }
}