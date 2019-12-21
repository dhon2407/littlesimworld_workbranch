using System;
using UnityEngine;
using System.Collections.Generic;

namespace InventorySystem
{
    [Serializable]
    public class ItemList : MonoBehaviour, IInteractable
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
        public float InteractionRange => 1;
        public Vector3 PlayerStandPosition => transform.position;
        public void Interact()
        {
            if (Inventory.Ready)
                Inventory.OpenCloseContainer(this, gameObject.name);
        }


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
