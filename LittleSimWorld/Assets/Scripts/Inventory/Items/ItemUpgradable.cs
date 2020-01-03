using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Upgradable Item", menuName = "Items/Upgradable")]
    public class ItemUpgradable : PassiveItem
    {
        public UpgradeType type;
        public int upgradeLevel;

        [Space]
        public ItemCode upgradeRequirement = ItemCode.NONE;

        [Space]
        public GameObject upgradesInto;

        public enum UpgradeType
        {
            BED,
            STOVE,
            FREEZER,
            DESK,
            SHOWER,
            TOILET,
            COOKTABLE,
        }
    }
}