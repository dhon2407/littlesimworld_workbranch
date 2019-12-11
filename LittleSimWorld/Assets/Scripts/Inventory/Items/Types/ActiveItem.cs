
using UnityEngine;

namespace InventorySystem
{
    public abstract class ActiveItem : ItemData
    {
        public override ItemType Type => ItemType.Active;
        [Space]
        public AudioClip UsageSound;
        public float UsageTime;
        public int cooldown;
        public string AnimationToPlayName = "None";

        public abstract void Use();
    }
}
