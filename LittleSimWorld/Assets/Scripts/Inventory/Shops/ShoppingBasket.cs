﻿using System.Collections.Generic;
using UnityEngine;
using Stats = PlayerStats.Stats;


namespace InventorySystem
{
    public class ShoppingBasket : MonoBehaviour
    {
        [SerializeField]
        protected Transform items = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI basketName = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI totalPriceUI = null;

        private float totalPrice = 0;
        private Color totalPriceInitialColor;

        public float TotalCost => totalPrice;
        public bool Empty => (items.childCount == 0);
        public List<ItemList.ItemInfo> Itemlist => CreateItemList();

        public void SetName(string newName)
        {
            basketName.text = newName;
        }

        public void ClearItems()
        {
            for (int i = 0; i < items.childCount; i++)
                Destroy(items.GetChild(i).gameObject);

            totalPrice = 0;
            UpdateTotalPriceDisplay();
        }

        public virtual void AddItem(ShopList.ItemInfo itemData)
        {
            if (Contains(itemData))
                Get(itemData).AddQuantity(1);
            else
                Shop.CreateBasketItem(items).
                    SetItem(Inventory.CreateItem(itemData.itemCode), itemData.price, 1).
                    SetAction(() => ReturnItem(itemData));

            UpdateGrandTotal();
        }

        protected void UpdateGrandTotal()
        {
            totalPrice = 0;
            foreach (var basketItem in GetCurrentItems())
                totalPrice += basketItem.Subtotal;

            UpdateTotalPriceDisplay();
        }

        protected void ReturnItem(ShopList.ItemInfo itemData)
        {
            if (Contains(itemData))
            {
                Get(itemData).RemoveQuantity(1);
                UpdateGrandTotal();
            }
        }

        public bool Contains(ShopList.ItemInfo itemData)
        {
            return GetCurrentItems().Exists(bItem => bItem.itemCode == itemData.itemCode);
        }

        protected BasketItem Get(ShopList.ItemInfo itemData)
        {
            return GetCurrentItems().Find(bItem => bItem.itemCode == itemData.itemCode);
        }

        private List<BasketItem> GetCurrentItems()
        {
            var basketItems = new List<BasketItem>();
            for (int i = 0; i < items.childCount; i++)
            {
                var basketItem = items.GetChild(i).GetComponent<BasketItem>();
                if (basketItem != null)
                    basketItems.Add(basketItem);
            }

            return basketItems;
        }

        private void Start()
        {
            totalPriceInitialColor = totalPriceUI.color;
            UpdateTotalPriceDisplay();
        }

        private void UpdateTotalPriceDisplay()
        {
            totalPriceUI.text = "£ " + totalPrice.ToString("0.00");
            totalPriceUI.color = (totalPrice > Stats.Money) ? Color.red : totalPriceInitialColor;
        }

        private List<ItemList.ItemInfo> CreateItemList()
        {
            var itemlist = new List<ItemList.ItemInfo>();
            foreach (var basketItem in GetCurrentItems())
            {
                itemlist.Add(new ItemList.ItemInfo
                {
                    itemCode = basketItem.itemCode,
                    count = basketItem.Qty
                });
            }

            return itemlist;
        }
    }
}