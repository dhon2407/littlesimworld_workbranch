using System.Collections.Generic;
using InventorySystem;
using PlayerStats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using static Cooking.Recipe.CookingHandler;

namespace Cooking.Recipe
{
    public class RecipeSlot : MonoBehaviour
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected Button button;

        [Header("Icon color tint")]
        [SerializeField] protected Color available = Color.white;
        [SerializeField] protected Color locked = new Color(200f / 255f, 200f / 255f, 200f / 255f, 128f / 255f);
        [SerializeField] protected Color hidden = new Color(10f / 255f, 10f / 255f, 10f / 255f, 10f / 255f);

        protected Item currentItem;
        protected Visibility visibility;
        private bool mouseOver;

        public Item Item => currentItem;
        
        public RecipeSlot SetItem(ItemCode recipeRecipeOutcome)
        {
            currentItem = Inventory.CreateItem(recipeRecipeOutcome);
            icon.sprite = currentItem.Icon;

            return this;
        }

        public void SetSelectAction(UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if ((visibility == Visibility.Available))
                    action.Invoke();
                else
                    CantCookThis();
            });
        }

        public void OnCursorEnter()
        {
            mouseOver = true;
            if (visibility != Visibility.Hidden)
                RecipeTooltip.Show(currentItem);
        }

        public void OnCursorExit()
        {
            mouseOver = false;
            RecipeTooltip.Hide();
        }

        protected void Reset()
        {
            button = GetComponent<Button>();
        }

        public RecipeSlot CheckRequirement()
        {
            var requiredItems = RecipeManager.GetItemRequirement(currentItem.Code);
            var slotRequirements = requiredItems.Count;

            if (RequiredLevel(slotRequirements) <= Stats.SkillLevel(Skill.Type.Cooking))
            {
                if (HaveEnoughIngredients(requiredItems))
                {
                    icon.color = available;
                    visibility = Visibility.Available;
                    button.interactable = true;

                    if (!SeenRecipes.Contains(currentItem.Code))
                        SeenRecipes.Add(currentItem.Code);

                    RefreshTooltip();
                }
                else if (CookedRecipes.Contains(currentItem.Code) ||
                         SeenRecipes.Contains(currentItem.Code))
                {
                    icon.color = locked;
                    visibility = Visibility.Locked;
                    button.interactable = true;

                    RefreshTooltip();
                }
                else
                {
                    icon.color = hidden;
                    visibility = Visibility.Hidden;
                    button.interactable = false;
                }
            }
            else
            {
                icon.color = hidden;
                visibility = Visibility.Hidden;
                button.interactable = false;
            }

            return this;
        }

        private void RefreshTooltip()
        {
            if (mouseOver)
                RecipeTooltip.Show(currentItem);
        }

        private bool HaveEnoughIngredients(List<Item> requiredItems)
        {
            var requirements = new Dictionary<ItemCode, int>();
            foreach (var requiredItem in requiredItems)
            {
                if (requirements.ContainsKey(requiredItem.Code))
                    requirements[requiredItem.Code]++;
                else
                    requirements[requiredItem.Code] = 1;
            }

            foreach (var requirement in requirements)
            {
                if (AvailableIngredients.Exists(ing => ing.itemCode ==requirement.Key) &&
                    AvailableIngredients.Find(ing => ing.itemCode == requirement.Key).count >= requirement.Value)
                    continue;

                return false;
            }

            return true;
        }

        private void CantCookThis()
        {
            throw new System.NotImplementedException();
        }

        public enum Visibility
        {
            Available,
            Hidden,
            Locked,
        }
    }
}
