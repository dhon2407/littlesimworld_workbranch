using UnityEngine;

using static PlayerStats.Skill;
using Stats = PlayerStats.Stats;

namespace InventorySystem
{
    public class ShoppingWindow : MonoBehaviour
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

        public void Open(ShopList shoplist)
        {
            if (!closed) return;

            ClearShopItems();
            ClearBasketItems();

            foreach (var itemData in shoplist.Items)
            {
                Shop.CreateShopItem(items).
                    SetItem(Inventory.CreateItem(itemData.itemCode), itemData.price).
                    SetAction(() => MoveToBasket(itemData));
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

            if (Stats.Money < basket.TotalCost)
            {
                GameLibOfMethods.CreateFloatingText("Not enough money", 2);
                return;
            }

            if (!Inventory.PlaceOnBag(basket.Itemlist))
            {
                GameLibOfMethods.CreateFloatingText("Not enough space in inventory", 2);
                return;
            }

            Stats.GetMoney(basket.TotalCost);
            Stats.AddXP(Type.Charisma, basket.TotalCost * 0.2f);

            ClearBasketItems();
            
            Inventory.SetBagItemActions(ItemActionOnShopping);
        }

        private void MoveToBasket(ShopList.ItemInfo itemData)
        {
            basket.AddItem(itemData);
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
            showScale = transform.localScale;
            Hide();
        }
    }
}