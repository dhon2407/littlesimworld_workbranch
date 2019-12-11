using System;
using UnityEngine;

namespace InventorySystem
{
    public class ItemContainer : Storage
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI containerName = null;

        public override void Open(ItemList data)
        {
            if (!closed) return;

            itemList = data;

            int currentSlot = 0;
            foreach (var itemData in data.Items)
            {
                Inventory.CreateSlot(itemCells[currentSlot].transform).
                    SetItem(Inventory.CreateItem(itemData), itemData.count).
                    SetSelfAction(Inventory.PlaceOnBag);
                    
                currentSlot++;
            }

            Show();
        }

        public override void Close()
        {
            if (closed) return;

            foreach (var item in itemCells)
            {
                var itemSlot = item.GetComponentInChildren<ItemSlot>();
                if (itemSlot != null)
                {
                    Debug.Log("Slot removed with item " + itemSlot.name);
                    Destroy(itemSlot.gameObject);
                }
            }

            Hide();
        }
        public void SetName(string name)
        {
            containerName.text = name;
        }
        public bool AddItem(ItemSlot itemSlot)
        {
            if (IsFull(itemSlot.CurrentItemCode))
                return false;

            if (Contains(itemSlot.CurrentItemCode))
                SlotOf(itemSlot.CurrentItemCode).Add(itemSlot);
            else
                NextEmptyCell().Move(itemSlot).SetSelfAction(Inventory.PlaceOnBag);

            Inventory.SFX(Inventory.Sound.Drop);

            return true;
        }

        protected override void SetupDropEvents()
        {
            foreach (var item in itemCells)
            {
                item.onDropEvent.AddListener(MoveToExistingItem);
                item.onDropEvent.AddListener(SetAction);
            }
        }

        private void SetAction(Droppable cell)
        {
            cell.GetSlot().SetSelfAction(Inventory.PlaceOnBag);
        }
    }
}