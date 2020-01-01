using Cooking.Recipe;
using UnityEngine;

namespace Cooking
{
    public class ManualCookHandler : MonoBehaviour
    {
        private static ManualCookHandler instance;

        [SerializeField] private RecipeLoader recipeLoader = null;
        [SerializeField] private CookList cookingList = null;

        [Space]
        [Header("Slot Cooking Lvl Requirements")]
        [SerializeField, Range(0, 10)] private int firstSlot = 0;
        [SerializeField, Range(0, 10)] private int secondSlot = 2;
        [SerializeField, Range(0, 10)] private int thirdSlot = 5;
        
        [Space]
        [SerializeField] private UIPopUp popUp = null;

        private bool isOpen = false;

        public bool Active => isOpen;

        public bool ToggleManualCooking()
        {
            isOpen = !isOpen;

            if (isOpen)
                Open();
            else
                Close();

            return isOpen;
        }

        public void Refresh()
        {
            recipeLoader.FetchRecipes();
            cookingList.ClearList();
        }

        public static int SlotRequiredLevel(int numberOfIngredients)
        {
            Mathf.Clamp(numberOfIngredients, 1, int.MaxValue);

            switch (numberOfIngredients)
            {
                case 1: return instance.firstSlot;
                case 2: return instance.secondSlot;
                case 3: return instance.thirdSlot;

                default:
                    return int.MaxValue;
            }
        }

        private void Open()
        {
            popUp.Open();
        }

        public void Close()
        {
            RecipeTooltip.Hide();
            popUp.Close();
            isOpen = false;

            cookingList.ReturnIngredients();
        }

        private void Awake()
        {
            instance = this;
        }
    }
}