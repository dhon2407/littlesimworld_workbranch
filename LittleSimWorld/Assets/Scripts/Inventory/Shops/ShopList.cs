using UnityEngine;
using System.Collections.Generic;
using System;

namespace InventorySystem
{
    public class ShopList : MonoBehaviour
    {
        [SerializeField]
        private List<ItemInfo> items = null;
        public int Count => items.Count;
        public List<ItemInfo> Items => items;

        [Serializable]
        public struct ItemInfo
        {
            public int price;
            public ItemCode itemCode;
        }
    }
}