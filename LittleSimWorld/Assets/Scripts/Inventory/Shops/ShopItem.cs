using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace InventorySystem
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField]
        new private TMPro.TextMeshProUGUI name = null;
        [SerializeField]
        protected TMPro.TextMeshProUGUI price = null;
        [SerializeField]
        private Image icon = null;
        [SerializeField]
        private Button actionButton = null;

        protected Item currentItem = null;

        public ItemCode itemCode => currentItem.Code;

        public ShopItem SetItem(Item item, float price)
        {
            currentItem = item;
            this.price.text = "£ " + price.ToString("0.00");
            name.text = currentItem.name;
            icon.sprite = currentItem.Data.icon;

            return this;
        }

        public void SetAction(UnityAction action)
        {
            actionButton.onClick.AddListener(action);
        }

        private void OnDestroy()
        {
            actionButton.onClick.RemoveAllListeners();
        }
    }
}