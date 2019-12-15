using System;
using UnityEngine;

namespace InventorySystem
{
    public class UpgradesShop : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI shopName = null;
        [SerializeField]
        private ShoppingBasket basket = null;
        [SerializeField]
        private Transform items = null;

        private bool closed;
        private Vector3 showScale;

        public void SetName(string name)
        {
            shopName.text = name;
        }

        public void Open(UpgradeList upgradeList)
        {
            if (!closed) return;

            ClearShopItems();
            ClearBasketItems();

            basket.SetName("Upgrade orders");

            foreach (var itemData in upgradeList.Items)
            {
                if (Upgrades.CanUpgradeTo(itemData.itemCode))
                {
                    Shop.CreateShopItem(items).
                        SetItem(Inventory.CreateItem(itemData.itemCode), itemData.price).
                        SetAction(() => MoveToBasket(itemData));
                }
            }

            Inventory.SetBagItemActions(ItemActionOnShopping);

            Show();
        }

        public void Close()
        {
            ClearShopItems();
            ClearBasketItems();

            Inventory.ResetBagItemActions();

            Hide();
        }

        public void Show()
        {
            transform.localScale = showScale;
            closed = false;
        }

        public void Hide()
        {
            transform.localScale = Vector3.zero;
            closed = true;
        }

        public void CheckOut()
        {
            if (basket.Empty) return;

            if (PlayerStatsManager.Instance.Money < basket.TotalCost)
            {
                GameLibOfMethods.CreateFloatingText("Not enough money", 2);
                return;
            }

            PlayerStatsManager.Instance.Money = Mathf.Clamp(PlayerStatsManager.Instance.Money - basket.TotalCost, 0, float.MaxValue);
            PlayerStatsManager.Instance.playerSkills[SkillType.Charisma].AddXP(basket.TotalCost * 0.2f);

            ExecuteUpgrades();

            Shop.CloseUpgradeShop();
        }

        private void ExecuteUpgrades()
        {
            var items = basket.Itemlist;
            foreach (var item in items)
                Upgrades.UpgradeItem(item.itemCode);
        }

        private void MoveToBasket(ShopList.ItemInfo itemData)
        {
            if (!basket.Contains(itemData))
                basket.AddItem(itemData);
            else
                GameLibOfMethods.CreateFloatingText("You already ordered this.", 1.5f);
        }

        private void ClearBasketItems()
        {
            basket.ClearItems();
        }

        private void ClearShopItems()
        {
            for (int i = 0; i < items.childCount; i++)
                Destroy(items.GetChild(i).gameObject);
        }

        private void ItemActionOnShopping(ItemSlot itemslot)
        {
            //DO NOTHING
        }

        private void Awake()
        {
            if (Upgrades.Ready) { }
            showScale = transform.localScale;
            Hide();
        }
    }
}