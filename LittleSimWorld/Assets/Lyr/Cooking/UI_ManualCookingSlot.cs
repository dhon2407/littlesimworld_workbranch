using System;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Cooking {
	public class UI_ManualCookingSlot : MonoBehaviour
    { 
        [SerializeField]
        private Image ingredientIcon = null;
        [SerializeField]
		private int requiredCookingLevel = 0;
        
        private ItemData currentItemInfo;

        public bool Empty => (currentItemInfo == null);
        public ItemCode ItemCode => currentItemInfo.code;

        [HideInInspector] public bool isAvailableForPlayer;

		public void CheckAvailability() {
			if (isAvailableForPlayer)
                return;

			isAvailableForPlayer = PlayerStatsManager.Instance.playerSkills[SkillType.Cooking].Level >= requiredCookingLevel;
		}

        public void PlaceItem(InventorySystem.ItemSlot itemSlot)
        {
            if (currentItemInfo != null)
                ReturnItemToInventory();

            currentItemInfo = itemSlot.CurrentItemData;
            ingredientIcon.sprite = currentItemInfo.icon;
            itemSlot.Consume(1);
        }

        public void ClearSlot()
        {
            ReturnItemToInventory();
            currentItemInfo = null;
            ingredientIcon.sprite = null;
        }

        private void ReturnItemToInventory()
        {
            if (currentItemInfo != null)
            {
                Inventory.PlaceOnBag(new List<ItemList.ItemInfo>()
                {
                    new ItemList.ItemInfo
                    {
                        itemCode = currentItemInfo.code,
                        count = 1
                    }
                });
            }
        }
    }
}