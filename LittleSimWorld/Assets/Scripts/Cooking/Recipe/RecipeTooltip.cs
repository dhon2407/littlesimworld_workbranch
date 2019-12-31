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
        [SerializeField] private TextMeshProUGUI recipeName = null;
        [SerializeField] private TextMeshProUGUI description = null;
        
        [Space]
        [SerializeField] private Transform ingredientsList = null;

        [Space]
        [Header("Screen Pointer offset")]
        [SerializeField]
        private Vector2 offset = Vector2.zero;
        
        private UIPopUp popUp;

        public void Show(Item recipe, List<Item> ingredients, bool sameItem = false)
        {
            ClearIngredients();

            recipeName.text = recipe.Data.name;
            description.text = recipe.Data.Description;

            for (int i = 0; i < ingredients.Count; i++)
            {
                var ingredientObject = ingredientsList.GetChild(i).gameObject;
                ingredientObject.SetActive(true);
                ingredientObject.GetComponent<IngredientTooltip>().SetItem(ingredients[i]);
            }

            UpdateIngredientsView();

            if (!sameItem)
                StartCoroutine(DelayOpen());
            else
                popUp.Open();
        }

        private IEnumerator DelayOpen()
        {
            GetComponent<VerticalLayoutGroup>().enabled = false;
            yield return null;
            GetComponent<VerticalLayoutGroup>().enabled = true;
            yield return null;

            popUp.Open();
        }

        public static void Show(Item currentItem, bool sameItem = false)
        {
            if (instance != null)
                instance.Show(currentItem, RecipeManager.GetItemRequirement(currentItem.Data.code), sameItem);
        }

        public void UpdateIngredientsView()
        {
            var previousIngredientsCount = new Dictionary<ItemCode, int>();
            
            for (int i = 0; i < instance.ingredientsList.childCount; i++)
            {
                var ingTooltip = instance.ingredientsList.GetChild(i).GetComponent<IngredientTooltip>();
                if (CookingHandler.AvailableIngredients.Exists(ing =>
                    ing.itemCode == ingTooltip.ItemCode))
                {
                    var availableIngredient =
                        CookingHandler.AvailableIngredients.Find(ing => ing.itemCode == ingTooltip.ItemCode);

                    if (previousIngredientsCount.ContainsKey(ingTooltip.ItemCode))
                    {
                        ingTooltip.GreyOut((availableIngredient.count - previousIngredientsCount[ingTooltip.ItemCode]) <= 0);

                        previousIngredientsCount[ingTooltip.ItemCode]++;
                    }
                    else
                    {
                        ingTooltip.GreyOut(availableIngredient.count == 0);

                        previousIngredientsCount[ingTooltip.ItemCode] = 1;
                    }
                }
                else
                {
                    ingTooltip.GreyOut(true);
                }
            }
        }

        public static void Hide()
        {
            instance.popUp.Close();
            instance.ClearIngredients();
        }

        private void ClearIngredients()
        {
            for (var i = 0; i < ingredientsList.childCount; i++)
                ingredientsList.GetChild(i).gameObject.SetActive(false);
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
            var position = (Vector2)(Input.mousePosition);
            var screenSize = new Vector2(Screen.width, Screen.height);

            return new Vector2(position.x + (screenSize.x * offset.x),
                position.y + (screenSize.y * offset.y));
        }
    }
}