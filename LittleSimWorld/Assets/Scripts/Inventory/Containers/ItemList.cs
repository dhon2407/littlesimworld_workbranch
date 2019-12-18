using System;
using UnityEngine;
using System.Collections.Generic;

namespace InventorySystem
{
    [Serializable]
    public class ItemList : MonoBehaviour
    {
        [SerializeField]
        private int containerID = 0;
        //[SerializeField] next feature
        private int maxSlots;
        [SerializeField]
        private List<ItemInfo> items = null;

        public int Count => items.Count;
        public List<ItemInfo> Items => items;
        public int ID => containerID;

        [Serializable]
        public struct ItemInfo
        {
            public int count;
            public ItemCode itemCode;
        }

        public void UpdateItems(List<ItemInfo> newItems)
        {
            items = newItems;
        }
    }
}
