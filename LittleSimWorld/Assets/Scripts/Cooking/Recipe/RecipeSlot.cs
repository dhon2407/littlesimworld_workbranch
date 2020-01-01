using InventorySystem;
using LSW.Helpers;
using PlayerStats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using static Cooking.ManualCookHandler;
using static Cooking.Recipe.CookingHandler;
using static Cooking.Recipe.RecipeSlot.Visibility;

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

        [Space]
        [Header("Shake Behaviour")]
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private float intensity = 0.02f;

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
                if (visibility == Available)
                    action.Invoke();
                else
                    CantCookThis();
            });
        }

        public void OnCursorEnter()
        {
            mouseOver = true;
            if (visibility != Hidden)
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

        public void CheckRequirement()
        {
            var requiredItems = RecipeManager.GetItemRequirement(currentItem.Code);
            var slotRequirements = requiredItems.Count;

            if (SlotRequiredLevel(slotRequirements) > Stats.SkillLevel(Skill.Type.Cooking))
            {
                SetVisibility(Hidden);
                return;
            }

            if (RecipeManager.HaveEnoughIngredients(currentRecipe, AvailableIngredients))
            {
                SetVisibility(Available);

                if (!SeenRecipes.Contains(currentItem.Code))
                    SeenRecipes.Add(currentItem.Code);
            }
            else
            {
                SetVisibility(CookedRecipes.Contains(currentItem.Code) ||
                              SeenRecipes.Contains(currentItem.Code) ?
                    Locked : Hidden);
            }
        }

        private void SetVisibility(Visibility value)
        {
            visibility = value;
            button.interactable = (visibility == Available ||
                                   visibility == Locked);

            if (button.interactable)
                RefreshTooltip();

            icon.color = GetIconColor();
        }

        private Color GetIconColor()
        {
            switch (visibility)
            {
                case Available: return available;
                case Locked: return locked;
                default: return hidden;
            }
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
            button.Shake(duration, intensity);
        }

        public enum Visibility
        {
            Available,
            Hidden,
            Locked,
        }
    }
}
