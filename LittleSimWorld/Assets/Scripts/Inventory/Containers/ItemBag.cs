using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    public class ItemBag : Storage
    {
        [SerializeField]
        private KeyCode functionKey = KeyCode.I;

        public int FreeSlot => GetFreeSlot();

        public override void Open(ItemList data) { }

        public override void Close() { }

        public bool AddItem(ItemSlot itemSlot)
        {
            if (IsFull(itemSlot.CurrentItemCode))
                return false;

            if (Contains(itemSlot.CurrentItemCode))
                SlotOf(itemSlot.CurrentItemCode).Add(itemSlot);
            else
                NextEmptyCell().Move(itemSlot).SetUseAction();

            Inventory.SFX(Inventory.Sound.Drop);

            return true;
        }

        public void AddItem(ItemList.ItemInfo itemInfo)
        {
            NextEmptyCell().SetItem(Inventory.CreateItem(itemInfo.itemCode), itemInfo.count).
                SetUseAction();
        }

        public bool CanFit(List<ItemList.ItemInfo> itemlist)
        {
            int slotToTake = itemlist.Count;

            foreach (var item in itemlist)
                if (Contains(item.itemCode))
                    slotToTake = Mathf.Clamp(slotToTake - 1, 0, itemlist.Count);

            return slotToTake <= FreeSlot;
        }

        public void AddItems(List<ItemList.ItemInfo> itemlist)
        {
            foreach (var item in itemlist)
            {
                if (Contains(item.itemCode))
                    SlotOf(item.itemCode).Add(item.count);
                else
                    AddItem(item);
            }
        }

        public void UpdateSlotActions(UnityAction<ItemSlot> slotAction)
        {
            foreach (var cell in itemCells)
                if (!cell.Empty)
                    cell.GetSlot().SetSelfAction(slotAction);
        }

        public void UpdateSlotUseActions()
        {
            foreach (var cell in itemCells)
                if (!cell.Empty)
                    cell.GetSlot().SetUseAction();
        }

        protected override void SetupDropEvents()
        {
            foreach (var cell in itemCells)
            {
                cell.onDropEvent.AddListener(EnableAction);
                cell.onDropEvent.AddListener(MoveToExistingItem);
            }
        }

        private void EnableAction(Droppable cell)
        {
            if (cell.Type != Droppable.CellType.TrashBin)
                cell.GetSlot().SetUseAction();
            else
                cell.GetSlot().ClearActions();
        }

        private int GetFreeSlot()
        {
            return itemCells.Count(cell => cell.Type != Droppable.CellType.TrashBin && cell.Empty);
        }

        private void Update()
        {
            if (!Inventory.Ready) return;
             
            if (Input.GetKeyDown(functionKey))
            {
                if (closed)
                    Show();
                else
                    Hide();
            }
        }
    }
}