using System.Collections.Generic;
using InventorySystem;
using UnityEngine;
using UnityEngine.UI;
using Cooking;

using static PlayerStats.Skill;
using Stats = PlayerStats.Stats;


namespace UI.Cooking {
	public class UI_ManualCookingSlot : MonoBehaviour
    { 
        [SerializeField]
        private Image ingredientIcon = null;
        [SerializeField]
        private Image slotBackground = null;
        [SerializeField]
		private int requiredCookingLevel = 0;
        [SerializeField]
        private IngredientCell cell = null;
        [SerializeField]
        private CharacterStats.StatData tooltip = null;

        private ItemData currentItemInfo;

        public bool Empty => (currentItemInfo == null);
        public ItemCode ItemCode => currentItemInfo.code;

        [HideInInspector] public bool isAvailableForPlayer;

		public void CheckAvailability() {
			if (isAvailableForPlayer)
                return;

			isAvailableForPlayer = Stats.Skill(Type.Cooking).CurrentLevel >= requiredCookingLevel;
		}

        public void GreyedOut(bool enable)
        {
            slotBackground.color = enable ? Color.gray : Color.white;
            cell.enabled = enable;
            ingredientIcon.enabled = enable;

            tooltip.description = enable ?
                "Place your ingredrient here." :
                "Raise your cooking level to unlock this.";
        }

        public void PlaceItem(InventorySystem.ItemSlot itemSlot)
        {
            if (currentItemInfo != null)
                ReturnItemToInventory();

            currentItemInfo = itemSlot.CurrentItemData;
            ingredientIcon.sprite = currentItemInfo.icon;
            itemSlot.Consume(1);

            tooltip.description = currentItemInfo.name;
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

                tooltip.description = "Place your ingredrient here.";
            }
        }
    }
}