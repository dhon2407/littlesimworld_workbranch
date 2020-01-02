using UnityEngine;
using System.Collections.Generic;
using InventorySystem;

namespace Cooking.Recipe
{
    public class CookList : MonoBehaviour
    {
        [SerializeField] private GameObject recipeSlot = null;
        [SerializeField] private Transform itemContainer = null;
        [SerializeField] private CookingHandler handler = null;
        [SerializeField] private RecipeLoader recipeLoader = null;

        private List<CookSlot> cookingSlots;
        private void Awake()
        {
            cookingSlots = new List<CookSlot>();
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

        public void ClearList()
        {
            cookingSlots.Clear();
            for (var i = 0; i < itemContainer.childCount; i++)
                Destroy(itemContainer.GetChild(i).gameObject);
        }

        public void ReturnIngredients()
        {
            if (cookingSlots.Count > 0)
            {
                foreach (var cookingSlot in cookingSlots)
                {
                    var requiredItems = RecipeManager.GetItemRequirement(cookingSlot.Item.Code);
                    foreach (var requiredItem in requiredItems)
                        handler.ReturnIngredient(requiredItem, cookingSlot.CurrentItemCount);
                }
            }
            
            ClearList();
        }

        public List<ItemList.ItemInfo> GetRecipesToCook()
        {
            var items = new List<ItemList.ItemInfo>();
            foreach (var cookingSlot in cookingSlots)
            {
                items.Add(new ItemList.ItemInfo
                {
                    itemCode = cookingSlot.Item.Code,
                    count = cookingSlot.CurrentItemCount,
                });
            }

            return items;
        }

        public void ManualCook()
        {
            if (cookingSlots.Count == 0) return;

            CookingEntity.ManualAction(GetRecipesToCook());
            ClearList();
        }
    }
}