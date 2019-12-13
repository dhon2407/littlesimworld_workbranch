using UnityEngine;
using System.Collections;

namespace InventorySystem
{
    public class UpgradeOrders : ShoppingBasket
    {
        public override void AddItem(ShopList.ItemInfo itemData)
        {
            if (!Contains(itemData))
                Shop.CreateUpgradeItem(items).
                    SetItem(Inventory.CreateItem(itemData.itemCode), itemData.price, 1).
                    SetAction(() => ReturnItem(itemData));

            UpdateGrandTotal();
        }
    }
}