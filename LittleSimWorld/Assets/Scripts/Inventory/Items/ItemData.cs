using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName ="Items/Basic")]
    public class ItemData : ScriptableObject
    {
        public ItemCode code;
        public new string name;
        public Sprite icon;
        public string Description;
        [Space]
        public bool isStackable;
        public int maxStack;
        [Space]
        public int price;
        
        public virtual ItemType Type => ItemType.Passive;

        public enum ItemType
        {
            Active,
            Passive
        }
    }
}