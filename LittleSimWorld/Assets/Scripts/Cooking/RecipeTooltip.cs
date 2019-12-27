using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking.Recipe
{
    public class RecipeTooltip : MonoBehaviour
    {
        private static RecipeTooltip instance;

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI recipeName;
        [SerializeField] private TextMeshProUGUI description;
        
        [Space]
        [SerializeField] private Transform ingredientsList;
        [SerializeField] private GameObject ingredient;
        
        [Space]
        [Header("Pointer offset")]
        [SerializeField]
        private Vector2 offset;
        
        private UIPopUp popUp;

        public void Show(Item recipe, List<Item> ingredients)
        {
            ClearIngredients();

            recipeName.text = recipe.Data.name;
            description.text = recipe.Data.Description;

            foreach (var item in ingredients)
                Instantiate(ingredient, ingredientsList)?.GetComponent<IngredientTooltip>().SetItem(item);

            UpdateIngredientsView();

            StartCoroutine(DelayOpen());
        }

        private IEnumerator DelayOpen()
        {
            GetComponent<VerticalLayoutGroup>().enabled = false;
            yield return null;
            GetComponent<VerticalLayoutGroup>().enabled = true;

            popUp.Open();
        }

        public static void Show(Item currentItem)
        {
            if (instance != null)
                instance.Show(currentItem, RecipeManager.GetItemRequirement(currentItem.Data.code));
        }

        public static void UpdateIngredientsView()
        {
            for (int i = 0; i < instance.ingredientsList.childCount; i++)
            {
                var ingTooltip = instance.ingredientsList.GetChild(i).GetComponent<IngredientTooltip>();
                if (CookingHandler.AvailableIngredients.Exists(ing =>
                    ing.itemCode == ingTooltip.ItemCode))
                {
                    ingTooltip.GreyOut(CookingHandler.AvailableIngredients.Find(ing =>
                        ing.itemCode == ingTooltip.ItemCode).count == 0);
                }
                else
                {
                    ingTooltip.GreyOut(true);
                }
            }
        }

        public static void Hide()
        {
            instance.ClearIngredients();
            instance.popUp.Close();
        }

        private void ClearIngredients()
        {
            for (var i = 0; i < ingredientsList.childCount; i++)
                Destroy(ingredientsList.GetChild(i).gameObject);
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            popUp = GetComponent<UIPopUp>();
        }

        private void Update()
        {
            if (popUp.Visible)
                transform.position = GetDisplayPosition();
        }

        private Vector2 GetDisplayPosition()
        {
            var position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var screenSize = new Vector2(Screen.width, Screen.height);

            return new Vector2(position.x + (offset.x / screenSize.x),
                position.y + (offset.y / screenSize.y));
        }
    }
}