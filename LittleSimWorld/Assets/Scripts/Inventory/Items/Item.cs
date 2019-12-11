using System;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private ItemData itemData = null;

        public Sprite Icon => itemData.icon;
        public string Name => itemData.name;
        public ItemCode Code => itemData.code;
        public ItemData Data => itemData;

        private void Awake()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.sprite = itemData.icon;
        }
    }
}