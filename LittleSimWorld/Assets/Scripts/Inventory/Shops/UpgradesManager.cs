using System.Collections.Generic;
using UnityEngine;

using UpgradeType = InventorySystem.ItemUpgradable.UpgradeType;

namespace InventorySystem
{
    public class UpgradesManager : MonoBehaviour
    {
        [SerializeField]
        private List<UpgradableItem> ItemGameObjects = null;
        [SerializeField]
        private List<UpgradableItemData> ItemUpgradeData = null;

        private Dictionary<UpgradeType, ItemCode> currentUpgrades;

        public Dictionary<UpgradeType, ItemCode> CurrentUpgrades => currentUpgrades;

        public void UpgradeItem(ItemCode itemCode)
        {
            if (currentUpgrades[GetUpgradeType(itemCode)] == itemCode)
                return;

            UpgradeType itemType = GetUpgradeType(itemCode);

            if (ItemGameObjects.Exists(itemObj => itemObj.type == itemType))
            {
                if (ItemUpgradeData.Exists(data => data.type == itemType))
                {
                    var tierlist = ItemUpgradeData.Find(data => data.type == itemType).itemsTier;
                    if (tierlist.Exists(tier => tier.code == itemCode))
                    {
                        ItemUpgradable newItemData = tierlist.Find(tier => tier.code == itemCode);
                        GameObject currentItemObject = ItemGameObjects.Find(itemObj => itemObj.type == itemType).targetItem;
                        GameObject newItemObject = Instantiate(newItemData.upgradesInto, currentItemObject.transform);

                        var oldObject = currentItemObject.transform.GetChild(0);
                        if (oldObject != null)
                            Destroy(oldObject.gameObject);

                        currentUpgrades[GetUpgradeType(itemCode)] = itemCode;

                        return;
                    }
                }
            }
            
            Debug.LogError(string.Format("No upgrade data for item type({0}) : item({1}).", itemType, itemCode));
        }

        public int GetCurrentUpgradeLevel(ItemCode itemCode)
        {
            return GetItemUpgradeLevel(currentUpgrades[GetUpgradeType(itemCode)]);
        }

        public ItemCode GetUpgradeRequirement(ItemCode itemCode)
        {
            var upgradeType = GetUpgradeType(itemCode);
            if (ItemUpgradeData.Exists(itemData => itemData.type == upgradeType))
            {
                var itemsTier = ItemUpgradeData.Find(itemData => itemData.type == upgradeType).itemsTier;
                if (itemsTier.Exists(item => item.code == itemCode))
                    return itemsTier.Find(item => item.code == itemCode).upgradeRequirement;
            }

            Debug.LogError(string.Format("Upgrade requirement for Item code({0}) details can't be found!", itemCode));
            return ItemCode.NONE;
        }

        public int GetItemUpgradeLevel(ItemCode itemCode)
        {
            var itemsTier = ItemUpgradeData.Find(itemData => itemData.type == GetUpgradeType(itemCode)).itemsTier;
            if (itemsTier.Exists(item => item.code == itemCode))
                return itemsTier.Find(item => item.code == itemCode).upgradeLevel;

            Debug.LogError(string.Format("Item code({0}) upgrade details can't be found!", itemCode));
            return int.MaxValue;
        }

        public UpgradeType GetUpgradeType(ItemCode itemCode)
        {
            foreach (var data in ItemUpgradeData)
                foreach (var item in data.itemsTier)
                    if (item.code == itemCode)
                        return data.type;

            throw new UnityException(string.Format("Item code {0} not on upgradable list", itemCode));
        }

        public bool SameRequirement(ItemCode itemCode)
        {
            var currentItemUpgrade = currentUpgrades[GetUpgradeType(itemCode)];
            var upgradeRequirement = GetUpgradeRequirement(itemCode);

            return currentItemUpgrade == upgradeRequirement;

        }

        public void UpdateUpgradesData(Dictionary<UpgradeType, ItemCode> data)
        {
            if (data != null)
                foreach (var itemCode in data.Values)
                    UpgradeItem(itemCode);
        }

        [System.Serializable]
        public struct UpgradableItem
        {
            public UpgradeType type;
            public GameObject targetItem;
        }

        [System.Serializable]
        public struct UpgradableItemData
        {
            public UpgradeType type;
            public List<ItemUpgradable> itemsTier;
        }

        private void Awake()
        {
            currentUpgrades = new Dictionary<UpgradeType, ItemCode>()
            {
                [UpgradeType.BED] = ItemCode.BED1,
                [UpgradeType.DESK] = ItemCode.DESK1,
                [UpgradeType.STOVE] = ItemCode.STOVE1,
                [UpgradeType.SHOWER] = ItemCode.SHOWER1,
                [UpgradeType.TOILET] = ItemCode.TOILET1,
            };

            Upgrades.SetManager(this);
        }
    }

    public static class Upgrades
    {
        private static UpgradesManager manager;

        public static bool Ready => (manager != null);
        public static Dictionary<UpgradeType, ItemCode> GetData => manager.CurrentUpgrades;

        public static void SetManager(UpgradesManager upgradesManager)
        {
            manager = upgradesManager;
        }

        public static void SetData(Dictionary<UpgradeType, ItemCode> data)
        {
            manager.UpdateUpgradesData(data);
        }

        public static int GetUpgradeLevel(ItemCode itemCode)
        {
            return manager.GetItemUpgradeLevel(itemCode);
        }

        public static bool CanUpgradeTo(ItemCode itemCode)
        {
            return manager.GetItemUpgradeLevel(itemCode) > manager.GetCurrentUpgradeLevel(itemCode) &&
                   manager.SameRequirement(itemCode);
        }

        public static void UpgradeItem(ItemCode itemCode)
        {
            manager.UpgradeItem(itemCode);
        }
    }
}