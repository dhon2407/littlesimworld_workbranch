using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public abstract class Storage : MonoBehaviour
    {
        public abstract void Open(ItemList data);
        public abstract void Close();
        public ItemList Itemlist => GetItemlist();
        public bool IsOpen => !closed;

        protected abstract void SetupDropEvents();
        protected List<Droppable> itemCells = new List<Droppable>();
        protected Vector3 showScale;
        protected ItemList itemList;
        protected bool closed;

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

        protected virtual void Awake()
        {
            itemCells = new List<Droppable>();
            showScale = transform.localScale;
            UpdateItemCells();
            Hide();
        }

        public bool IsFull(ItemCode currentItemCode)
        {
            foreach (var item in itemCells)
            {
                if (item.Type == Droppable.CellType.TrashBin)
                    continue;

                if (item.Empty || currentItemCode == item.GetSlot().CurrentItemCode)
                    return false;
            }

            return true;
        }

        protected virtual void Start()
        {
            SetupDropEvents();
        }

        protected void MoveToExistingItem(Droppable cell)
        {
            var itemSlot = cell.GetSlot();
            var containingCells = FindCells(itemSlot.CurrentItemCode);

            foreach (var containingCell in containingCells)
                if (!containingCell.Equals(cell))
                    itemSlot.Add(containingCell.GetSlot());
        }

        protected List<Droppable> FindCells(ItemCode itemCode)
        {
            var cellsContaining = new List<Droppable>();
            foreach (var cell in itemCells)
            {
                if (cell.Type == Droppable.CellType.TrashBin)
                    continue;

                var itemSlot = cell.GetSlot();

                if (!cell.Empty && itemCode == itemSlot.CurrentItemCode &&
                    itemSlot.Stackable && !itemSlot.MaxStack)
                    cellsContaining.Add(cell);
            }

            return cellsContaining;
        }

        protected bool Contains(ItemCode itemCode)
        {
            foreach (var cell in itemCells)
            {
                if (cell.Type == Droppable.CellType.TrashBin)
                    continue;

                var itemSlot = cell.GetSlot();

                if (!cell.Empty && itemCode == itemSlot.CurrentItemCode &&
                    itemSlot.Stackable && !itemSlot.MaxStack)
                    return true;
            }

            return false;
        }

        protected ItemSlot SlotOf(ItemCode itemCode)
        {
            foreach (var item in itemCells)
            {
                if (item.Type == Droppable.CellType.TrashBin)
                    continue;

                if (!item.Empty && itemCode == item.GetSlot().CurrentItemCode)
                    return item.GetSlot();
            }

            return null;
        }

        protected ItemSlot NextEmptyCell()
        {
            foreach (var item in itemCells)
            {
                if (item.Type == Droppable.CellType.TrashBin)
                    continue;

                if (item.Empty)
                    return Inventory.CreateSlot(item.transform);
            }

            throw new UnityException("No available bag slot.");
        }

        private void UpdateItemCells()
        {
            itemCells.Clear();
            foreach (var cell in GetComponentsInChildren<Droppable>())
                itemCells.Add(cell);
        }

        private ItemList GetItemlist()
        {
            itemList.Items.Clear();

            foreach (var cell in itemCells)
            {
                if (cell.Type == Droppable.CellType.TrashBin)
                    continue;

                if (!cell.Empty)
                {
                    var itemSlot = cell.GetSlot();
                    itemList.Items.Add(new ItemList.ItemInfo
                    {
                        itemCode = itemSlot.CurrentItemCode,
                        count = itemSlot.Quantity
                    });
                }
            }

            return itemList;
        }

    }
}