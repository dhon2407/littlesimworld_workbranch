using UnityEngine;
using System.Collections.Generic;
using System;

namespace InventorySystem
{
	public class ShopList : MonoBehaviour, IInteractable {
        [SerializeField]
        private List<ItemInfo> items = null;
        public int Count => items.Count;
        public List<ItemInfo> Items => items;

		public float InteractionRange => 1;
		public Vector2 PlayerStandPosition => transform.position;

		public virtual void Interact() { if (Shop.Ready) { Shop.OpenCloseShop(this, gameObject.name); } }

		[Serializable]
        public struct ItemInfo
        {
            public int price;
            public ItemCode itemCode;
        }
    }
}