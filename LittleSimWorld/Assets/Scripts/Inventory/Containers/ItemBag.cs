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
        private UnityAction<ItemSlot> openBagSlotAction;

        public int FreeSlot => GetFreeSlot();

        public override void Close() { }

        public override void Open(ItemList data)
        {
            if (data == null)
                return;

            itemList = data;

            int currentSlot = 0;
            foreach (var itemData in data.Items)
            {
                Inventory.CreateSlot(itemCells[currentSlot].transform).
                    SetItem(Inventory.CreateItem(itemData), itemData.count).
                    SetSelfAction(Inventory.PlaceOnBag);

                currentSlot++;
            }
        }

        public void InitializeItems(List<ItemList.ItemInfo> list)
        {
            if (list == null)
                return;

            itemList.UpdateItems(list);

            Open(itemList);
        }

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
            int maxStack = Inventory.CreateItem(itemInfo.itemCode).Data.maxStack;
            int remainingCount = itemInfo.count;

            while (remainingCount > 0)
            {
                int bundleCount = Mathf.Clamp(remainingCount, 0, maxStack);
                var newItem = NextEmptyCell().SetItem(Inventory.CreateItem(itemInfo.itemCode), bundleCount);
                if (openBagSlotAction != null)
                    newItem.SetSelfAction(openBagSlotAction);
                else
                    newItem.SetUseAction();

                remainingCount -= bundleCount;
            }
        }

        public bool CanFit(List<ItemList.ItemInfo> itemlist)
        {
            int slotToTake = 0;

            foreach (var item in itemlist)
            {
                slotToTake++;
                int maxStack = Inventory.CreateItem(item.itemCode).Data.maxStack;
                if (item.count > maxStack)
                {
                    int extra = item.count - maxStack;
                    slotToTake += Mathf.CeilToInt((float)extra / maxStack);
                }
            }

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

        public void PickupItem(Item item)
        {
            if (IsFull(item.Code) && !Contains(item.Code))
            {
                GameLibOfMethods.CreateFloatingText("Not enough space in inventory", 2);
            }
            else
            {
                AddItem(new ItemList.ItemInfo
                {
                    itemCode = item.Code,
                    count = item.Count
                });

                Destroy(item.gameObject);
            }
                
        }

        public void RemoveItems(List<ItemList.ItemInfo> itemlist)
        {
            foreach (var item in itemlist)
            {
                if (Contains(item.itemCode))
                    SlotOf(item.itemCode).Consume(item.count);
            }
        }

        public void UpdateSlotActions(UnityAction<ItemSlot> slotAction)
        {
            openBagSlotAction = slotAction;
            foreach (var cell in itemCells)
                if (!cell.Empty)
                    cell.GetSlot().SetSelfAction(slotAction);
        }

        public void UpdateSlotUseActions()
        {
            openBagSlotAction = null;
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
            var cellSlot = cell.GetSlot();
            if (cell.Type == Droppable.CellType.TrashBin)
                cellSlot.ClearActions();
            else if (Inventory.ContainerOpen || Shop.IsShopOpen || _CookingStove.instance.Open)
                cellSlot.SetSelfAction(openBagSlotAction);
            else
                cellSlot.SetUseAction();
        }

        private int GetFreeSlot()
        {
            return itemCells.Count(cell => cell.Type != Droppable.CellType.TrashBin && cell.Empty);
        }

        protected override void Start()
        {
            base.Start();
            Show();
        }

        protected override void Awake()
        {
            base.Awake();
            itemList = GetComponent<ItemList>();
            Show();
        }

        private void Update()
        {
#if ITEM_INVENTORY_HIDE_SUPPORTED
            if (!Inventory.Ready) return;

            if (Input.GetKeyDown(functionKey))
            {
                if (closed)
                    Show();
                else
                    Hide();
            }
#endif
        }
    }
}