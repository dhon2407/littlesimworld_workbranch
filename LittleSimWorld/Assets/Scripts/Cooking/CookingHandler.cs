using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;

namespace Cooking.Recipe
{
    public class CookingHandler : MonoBehaviour
    {
        private static CookingHandler instance;

        private const int freezerID = 0;
        private List<ItemList.ItemInfo> availableIngredients;
        private List<ItemCode> cookedRecipes;
        private List<ItemCode> seenRecipes;

        [Header("Slot Level Requirements")]
        [SerializeField, Range(0, 10)]private int firstSlot = 0;
        [SerializeField, Range(0, 10)] private int secondSlot = 2;
        [SerializeField, Range(0, 10)] private int thirdSlot = 5;

        public static List<ItemList.ItemInfo> AvailableIngredients => instance.availableIngredients;
        public static List<ItemCode> CookedRecipes => instance.cookedRecipes;
        public static List<ItemCode> SeenRecipes => instance.seenRecipes;
        public bool Initialized { get; private set; }

        public void UpdateIngredientSource()
        {
            availableIngredients = new List<ItemList.ItemInfo>();
            AddIngredientSource(Inventory.BagItems);
            AddIngredientSource(Inventory.GetContainerItems(freezerID));

            Initialized = true;
        }

        public void AddIngredientSource(List<ItemList.ItemInfo> list)
        {
            if (list == null) return;
            
            foreach (var info in list)
            {
                if (availableIngredients.Exists(itemInfo => itemInfo.itemCode == info.itemCode))
                {
                    var ingredient = availableIngredients.Find(itemInfo => itemInfo.itemCode == info.itemCode);
                    ingredient.count += info.count;
                    continue;
                }

                availableIngredients.Add(info);
            }
        }

        public static int RequiredLevel(int numberOfIngredients)
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

        public static void SetCookedRecipes(List<ItemCode> savedCookedRecipes)
        {
            instance.InitializeCookedItems(savedCookedRecipes);
        }

        public void TakeIngredient(Item requiredItem)
        {
            for (int i = 0; i < availableIngredients.Count; i++)
            {
                if (availableIngredients[i].itemCode == requiredItem.Code)
                {
                    var ing = availableIngredients[i];
                    ing.count = Mathf.Clamp(ing.count - 1, 0, int.MaxValue);
                    availableIngredients[i] = ing;
                    return;
                }
            }
        }

        public void ReturnIngredient(Item requiredItem)
        {
            for (int i = 0; i < availableIngredients.Count; i++)
            {
                if (availableIngredients[i].itemCode == requiredItem.Code)
                {
                    var ing = availableIngredients[i];
                    ing.count = Mathf.Clamp(ing.count + 1, 0, int.MaxValue);
                    availableIngredients[i] = ing;
                    return;
                }
            }

            availableIngredients.Add(new ItemList.ItemInfo {itemCode = requiredItem.Code, count = 1});
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Inventory.Ready || !Inventory.Initialized || cookedRecipes == null)
                yield return null;

            #region TEST: Inject initial items

#if UNITY_EDITOR
            Inventory.PlaceOnBag(new List<ItemList.ItemInfo>
            {
                new ItemList.ItemInfo {itemCode = ItemCode.FISH, count = 2},
            });
#endif

            #endregion

            UpdateIngredientSource();

            //TODO: Should be refreshed when opening manual view
            seenRecipes = new List<ItemCode>();
        }

        private void InitializeCookedItems(List<ItemCode> savedCookedRecipes)
        {
            cookedRecipes = savedCookedRecipes ?? new List<ItemCode>();
        }
    }
}
