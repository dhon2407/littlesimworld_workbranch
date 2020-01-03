using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using TMPro;
using UnityEngine.UI;

namespace Cooking.Recipe
{
    public class CookingHandler : MonoBehaviour
    {
        private static CookingHandler instance;

        private const int freezerID = 0;
        private List<ItemList.ItemInfo> availableIngredients;
        private List<ItemCode> cookedRecipes;
        private List<ItemCode> seenRecipes;
        private bool isOpen = false;
        private Vector2 stoveLocation;

        private AutoCookHandler autoCook;

        [Header("Canvas Ray Caster")]
        [SerializeField] private GraphicRaycaster cookingCanvas = null;

        [Space]
        [Header("Action Buttons")]
        [SerializeField, FieldLabel("Auto / Continue")] private Button auto_continue = null;
        [SerializeField, FieldLabel("Manual / Reset")] private Button manual_reset = null;

        [Space]
        [SerializeField, FieldLabel("Manual Cooking Handler")] private ManualCookHandler manualCook = null;

        [Space]
        [SerializeField] private UIPopUp popUp = null;

        public static List<ItemList.ItemInfo> AvailableIngredients => instance.availableIngredients;
        public static List<ItemCode> CookedRecipes => instance.cookedRecipes;
        public static List<ItemCode> SeenRecipes => instance.seenRecipes;
        public static List<ItemList.ItemInfo> AutoCookItem => instance.GetAutoCookItem();
        public static bool Ongoing => instance.isOpen;

        public static bool EnableCanvas { set => instance.cookingCanvas.enabled = value; }

        public static void SetCookedRecipes(List<ItemCode> savedCookedRecipes)
        {
            instance.InitializeCookedItems(savedCookedRecipes);
        }

        public static void ToggleView(Vector2 stoveLocation, bool newCook)
        {
            if (instance.isOpen)
                instance.Close();
            else
                instance.Open(stoveLocation, newCook);
        }

        public static void ForceClose()
        {
            instance.Close();
        }

        public static void AddCookedRecipes(List<ItemList.ItemInfo> itemsToCook)
        {
            foreach (var itemInfo in itemsToCook)
                instance.cookedRecipes.Add(itemInfo.itemCode);
        }

        public void UpdateIngredientSource()
        {
            availableIngredients = new List<ItemList.ItemInfo>();
            AddIngredientSource(Inventory.BagItems);
            AddIngredientSource(Inventory.GetContainerItems(freezerID));

            manualCook.Refresh();
        }

        public void AddIngredientSource(List<ItemList.ItemInfo> list)
        {
            if (list == null) return;

            foreach (var availableItem in list)
                AddToAvailableIngredients(availableItem);
        }

        public void ToggleManualCooking()
        {
            const float horizontalOffset = -700f;
            var manualCooking = manualCook.ToggleManualCooking();
            
            popUp.Move(manualCooking ? new Vector2(horizontalOffset, 0) : popUp.PopInPosition);
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

                    TakeItemFromPlayer(requiredItem.Code);

                    return;
                }
            }
        }

        public void ReturnIngredient(Item requiredItem, int qty = 1)
        {
            var itemInfo = new ItemList.ItemInfo {itemCode = requiredItem.Code, count = qty };
            
            AddToAvailableIngredients(itemInfo);
            ReturnItemToPlayer(itemInfo);
        }

        public void Close()
        {
            EnableCanvas = false;
            manualCook.Close();

            popUp.Close(stoveLocation,() => isOpen = false);
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            autoCook = new AutoCookHandler();
        }

        private void Start()
        {
            seenRecipes = new List<ItemCode>();
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
                new ItemList.ItemInfo {itemCode = ItemCode.BREAD, count = 2},
                new ItemList.ItemInfo {itemCode = ItemCode.VEGETABLE, count = 2},
                new ItemList.ItemInfo {itemCode = ItemCode.MEAT, count = 2},
            });
#endif

            #endregion

            UpdateIngredientSource();
        }

        private void InitializeCookedItems(List<ItemCode> savedCookedRecipes)
        {
            cookedRecipes = savedCookedRecipes ?? new List<ItemCode>();
        }

        private void Open(Vector2 stoveLocation, bool newCook)
        {
            this.stoveLocation = stoveLocation; 
            seenRecipes = new List<ItemCode>();

            if (newCook)
                StartCooking();
            else
                ResumeCooking();

            popUp.Open(stoveLocation, () =>
            {
                isOpen = true;
                EnableCanvas = true;
            });
        }

        private void StartCooking()
        {
            CookingEntity.ResetAction();
            UpdateIngredientSource();
            auto_continue.onClick.RemoveAllListeners();
            auto_continue.onClick.AddListener(AutoCook);
            auto_continue.GetComponentInChildren<TextMeshProUGUI>().text = CookingEntity.AutoActionText;
            manual_reset.onClick.RemoveAllListeners();
            manual_reset.onClick.AddListener(ToggleManualCooking);
            manual_reset.GetComponentInChildren<TextMeshProUGUI>().text = CookingEntity.ManualActionText;
        }


        private void ResumeCooking()
        {
            auto_continue.onClick.RemoveAllListeners();
            auto_continue.onClick.AddListener(CookingEntity.Resume);
            auto_continue.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            manual_reset.onClick.RemoveAllListeners();
            manual_reset.onClick.AddListener(StartNewCook);
            manual_reset.GetComponentInChildren<TextMeshProUGUI>().text = "Start again";
        }

        private void StartNewCook()
        {
            popUp.ReOpen();
            StartCooking();
        }
        private void AutoCook()
        {
            if (manualCook.Active)
                manualCook.Close();

            CookingEntity.AutoAction();
        }

        private void AddToAvailableIngredients(ItemList.ItemInfo availableItem)
        {
            for (int i = 0; i < availableIngredients.Count; i++)
            {
                if (availableIngredients[i].itemCode == availableItem.itemCode)
                {
                    var ingredient = availableIngredients[i];
                    ingredient.count += availableItem.count;
                    availableIngredients[i] = ingredient;
                    return;
                }
            }

            availableIngredients.Add(availableItem);
        }

        private void TakeItemFromPlayer(ItemCode requiredItemCode)
        {
            if (Inventory.BagItems.Exists(itemInfo => itemInfo.itemCode == requiredItemCode))
            {
                Inventory.RemoveInBag(new List<ItemList.ItemInfo>
                {
                    new ItemList.ItemInfo {itemCode = requiredItemCode, count = 1}
                });

                return;
            }

            var fridgeItems = Inventory.GetContainerItems(freezerID);

            if (fridgeItems == null) return;

            for (int i = 0; i < fridgeItems.Count; i++)
            {
                if (fridgeItems[i].itemCode == requiredItemCode)
                {
                    var itemInfo = fridgeItems[i];
                    itemInfo.count -= 1;

                    if (itemInfo.count <= 0)
                        fridgeItems.RemoveAt(i);
                    else
                        fridgeItems[i] = itemInfo;
                }
            }
        }

        private void ReturnItemToPlayer(ItemList.ItemInfo itemInfo)
        {
            if (Inventory.CanFitOnBag(new List<ItemList.ItemInfo> {itemInfo}))
            {
                Inventory.PlaceOnBag(new List<ItemList.ItemInfo> {itemInfo});
                return;
            }

            var fridgeItems = Inventory.GetContainerItems(freezerID);
            
            if (fridgeItems == null) return;
            
            for (int i = 0; i < fridgeItems.Count; i++)
            {
                if (fridgeItems[i].itemCode == itemInfo.itemCode)
                {
                    var item = fridgeItems[i];
                    item.count += itemInfo.count;
                    fridgeItems[i] = item;
                    return;
                }
            }

            fridgeItems.Add(itemInfo);
        }

        private List<ItemList.ItemInfo> GetAutoCookItem()
        {
            var itemToCook = autoCook.GetItem();

            if (itemToCook != CookingEntity.DefaultCookItem)
                instance.TakeIngredientsFromPlayer(itemToCook);

            return new List<ItemList.ItemInfo>
            {
                new ItemList.ItemInfo {itemCode = itemToCook, count = 1}
            };
        }

        private void TakeIngredientsFromPlayer(ItemCode itemCodeToCook)
        {
            foreach (var item in RecipeManager.GetItemRequirement(itemCodeToCook))
                TakeItemFromPlayer(item.Code);
        }
    }
}
