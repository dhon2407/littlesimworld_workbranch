using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

namespace InventorySystem
{
    public class ItemSlot : MonoBehaviour
    {
        private const ItemData.ItemType ActiveItem = ItemData.ItemType.Active;

        [SerializeField]
        private TMPro.TextMeshProUGUI quantity = null;
        [SerializeField]
        private Image icon = null;
        [SerializeField]
        private Image coolDownImage = null;
        [SerializeField]
        private Button button = null;

        private Item itemInside;
        private int currentQty;
        private bool onCoolDown;

        public ItemCode CurrentItemCode => itemInside.Code;
        public ItemData CurrentItemData => itemInside.Data;
        public int Quantity => currentQty;

        public void SetClickAction(UnityAction action)
        {
            ClearActions();
            button.onClick.AddListener(action);
        }

        public void SetSelfAction(UnityAction<ItemSlot> action)
        {
            ClearActions();
            button.onClick.AddListener(()=> action(this));
        }

        public void SetUseAction()
        {
            ClearActions();
            button.onClick.AddListener(UseItem);
        }

        public void ClearActions()
        {
            button.onClick.RemoveAllListeners();
        }

        public bool Same(ItemSlot item)
        {
            return (CurrentItemCode == item.CurrentItemCode);
        }

        public ItemSlot Move(ItemSlot itemSlot)
        {
            itemInside = itemSlot.itemInside;
            currentQty = itemSlot.currentQty;

            RefreshItem();

            itemSlot.Delete();

            return this;
        }

        private void RefreshItem()
        {
            name = itemInside.name;
            icon.sprite = itemInside.Data.icon;
            UpdateQtyDisplay();
            UpdateTooltip();
        }

        public void Add(ItemSlot newItemSlot)
        {
            currentQty += newItemSlot.currentQty;
            UpdateQtyDisplay();
            newItemSlot.Delete();
        }

        public void Add(int quantity)
        {
            currentQty += quantity;
            UpdateQtyDisplay();
        }

        public ItemSlot SetItem(Item item, int quantity)
        {
            itemInside = item;
            currentQty = quantity;

            RefreshItem();

            return this;
        }

        public void Delete()
        {
            Destroy(gameObject);
        }

        private void UseItem()
        {
            if (itemInside.Data.Type == ActiveItem && !onCoolDown)
            {
                ActiveItem itemParams = (ActiveItem)itemInside.Data;
                if (!GameLibOfMethods.animator.GetBool("Walking"))
                {
                    GameLibOfMethods.StaticCoroutine.Start(GameLibOfMethods.DoAction(
                        delegate
                        {
                            itemParams.Use();
                            Consume(1);
                            StartCoroutine(StartCooldown(itemParams.cooldown));
                        },
                        itemParams.UsageTime,
                        null,
                        itemParams.AnimationToPlayName));

                    Inventory.FoodInHand = itemParams.icon;
                }
            }
        }

        private void Consume(int amount)
        {
            currentQty -= amount;

            if (currentQty <= 0)
                Destroy(gameObject);
            else
                UpdateQtyDisplay();
        }

        private IEnumerator StartCooldown(float cooldownTime)
        {
            if (currentQty <= 0) yield return null;

            onCoolDown = true;
            float timeLapse = 0;

            while (timeLapse < cooldownTime)
            {
                timeLapse += Time.deltaTime;
                coolDownImage.fillAmount = Mathf.Lerp(1, 0, timeLapse / cooldownTime);
                yield return null;
            }

            coolDownImage.fillAmount = 0;
            onCoolDown = false;
        }

        private void UpdateQtyDisplay()
        {
            quantity.text = (currentQty > 1) ? currentQty.ToString("0") : string.Empty;
            quantity.transform.parent.localScale = (quantity.text == string.Empty) ? Vector3.zero : Vector3.one;
        }

        private void UpdateTooltip()
        {
            var tooltip = GetComponent<TooltipArea>();
            if (tooltip != null)
                tooltip.SetDisplay(itemInside.Data.name, itemInside.Data.Description);
        }

        private void Start()
        {
            button.onClick.AddListener(ItemToolTip.Hide);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
            ItemToolTip.Hide();
        }
    }
}