using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName ="Items/Basic")]
    public class ItemData : ScriptableObject
    {
        public ItemCode code;
        public ItemKind kind;
        public new string name;
        public Sprite icon;
        public string Description;
        [Space]
        public bool isStackable;
        public int maxStack;
        [Space]
        public int price;
        
        public virtual ItemState State => ItemState.Passive;

        public enum ItemState
        {
            Active,
            Passive,
        }

        public enum ItemKind
        {
            None,
            Ingredient,
            Furniture,
        }
    }
}