using InventorySystem;
using PlayerStats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using static Cooking.ManualCookHandler;
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
        [SerializeField] protected Color hidden = new Color(10f / 255f, 10f / 255f, 10f / 255f, 70f / 255f);

        protected Item currentItem;
        protected NewRecipe currentRecipe;
        protected Visibility visibility;
        private bool mouseOver;
        private bool hasGameFocus = true;

        public Item Item => currentItem;
        
        public RecipeSlot SetRecipe(NewRecipe recipe)
        {
            currentRecipe = recipe;
            currentItem = Inventory.CreateItem(recipe.RecipeOutcome);
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

            if (SlotRequiredLevel(slotRequirements) <= Stats.SkillLevel(Skill.Type.Cooking))
            {
                if (RecipeManager.HaveEnoughIngredients(currentRecipe, AvailableIngredients))
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
                RecipeTooltip.Show(currentItem, true);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            hasGameFocus = hasFocus;
            if (!hasFocus)
                RecipeTooltip.Hide();
            else if (mouseOver)
                CheckRequirement();
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
