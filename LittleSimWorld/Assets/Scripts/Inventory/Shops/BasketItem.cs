using System;
using UnityEngine;

namespace InventorySystem
{
    public class BasketItem : ShopItem
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI quantity = null;
        private int currentQty;
        private float unitPrice;

        public float Subtotal => unitPrice * currentQty;
        public int Qty => currentQty;

        public BasketItem SetItem(Item item, float price, int qty)
        {
            SetItem(item, price);
            SetQuantity(qty);
            unitPrice = price;

            UpdateTotalPrice();
            UpdateTooltip();

            return this;
        }

        public void SetQuantity(int qty)
        {
            currentQty = qty;
            UpdateQtyDisplay();
        }

        public void AddQuantity(int amount)
        {
            currentQty += amount;
            UpdateQtyDisplay();
        }

        public void RemoveQuantity(int amount)
        {
            currentQty -= amount;

            if (currentQty <= 0)
                Destroy(gameObject);
            else
                UpdateQtyDisplay();
        }

        private void UpdateTotalPrice()
        {
            price.text = "£ " + (unitPrice * currentQty).ToString("0.00");
        }

        private void UpdateQtyDisplay()
        {
            quantity.text = currentQty.ToString("0");
            UpdateTotalPrice();
        }

        private void UpdateTooltip()
        {
            var tooltip = GetComponentInChildren<TooltipArea>();
            if (tooltip != null)
                tooltip.SetDisplay(currentItem.Data.name, currentItem.Data.Description);
        }
    }
}