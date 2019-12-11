using UnityEngine;
using System.Collections.Generic;
using System;

namespace InventorySystem
{
    public class ItemList : MonoBehaviour
    {
        //[SerializeField] next feature
        private int maxSlots;
        [SerializeField]
        private List<ItemInfo> items = null;

        public int Count => items.Count;
        public List<ItemInfo> Items => items;

        [Serializable]
        public struct ItemInfo
        {
            public int count;
            public ItemCode itemCode;
        }
    }
}
