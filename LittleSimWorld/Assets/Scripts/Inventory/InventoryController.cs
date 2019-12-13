using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryController : MonoBehaviour
    {
        private static InventoryController instance;

        private static ItemBag playerInventory = null;
        private static ItemContainer currentContainer = null;
        private static ItemList currentContainerData = null;
        private bool containerOpen;

        public bool ContainerOpen => containerOpen;

        [SerializeField]
        private Transform canvasTransform = null;
        [SerializeField]
        private AudioSource audioSource = null;

        [Header("SFX")]
        [SerializeField]
        private AudioClip startDragSound = null;
        [SerializeField]
        private AudioClip cancelDragSound = null;
        [SerializeField]
        private AudioClip dropSound = null;

        [Space]
        [SerializeField]
        private SpriteRenderer foodInHand = null;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            Inventory.SetController(this);
        }

        public void OpenContainer(ItemList items, string name)
        {
            containerOpen = true;
            currentContainerData = items;
            currentContainer.SetName(name);
            currentContainer.Open(items);

            playerInventory.UpdateSlotActions(PutInCurrentContainer);

            ShowBag();
        }

        public void CloseContainer()
        {
            currentContainerData = currentContainer.Itemlist;
            currentContainer.Close();
            currentContainerData = null;
            containerOpen = false;

            playerInventory.UpdateSlotUseActions();

            HideBag();
        }

        public void PutInBag(ItemSlot itemSlot)
        {
            playerInventory.AddItem(itemSlot);

            if (containerOpen)
                playerInventory.UpdateSlotActions(PutInCurrentContainer);
        }

        public void PutInBag(List<ItemList.ItemInfo> itemlist)
        {
            ShowBag();
            playerInventory.AddItems(itemlist);

            if (containerOpen)
                playerInventory.UpdateSlotActions(PutInCurrentContainer);
        }

        public void RemoveInBag(List<ItemList.ItemInfo> itemlist)
        {
            playerInventory.RemoveItems(itemlist);
        }

        public void PlaySFX(Inventory.Sound sound)
        {
            AudioClip clip = GetSoundClip(sound);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }

        }

        public void SetFoodInHand(Sprite sprite)
        {
            foodInHand.sprite = sprite;
        }

        public bool CanFitOnBag(List<ItemList.ItemInfo> itemlist)
        {
            return playerInventory.CanFit(itemlist);
        }

        public void ShowBag()
        {
            playerInventory.Show();
        }

        public void HideBag()
        {
            playerInventory.Hide();
        }

        public void SetBagItemActions(UnityEngine.Events.UnityAction<ItemSlot> action)
        {
            playerInventory.UpdateSlotActions(action);
        }

        public void ResetBagActions()
        {
            playerInventory.UpdateSlotUseActions();
        }

        private void PutInCurrentContainer(ItemSlot itemSlot)
        {
            if (currentContainer.IsOpen)
                currentContainer.AddItem(itemSlot);
            else
                Debug.LogWarning("No open container.");

        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (containerOpen)
            {
                var container = GameLibOfMethods.CheckInteractable()?.GetComponent<ItemList>();
                if (container == null || !container.Equals(currentContainerData))
                    CloseContainer();
            }
        }


        private void Initialize()
        {
            playerInventory = Instantiate(Resources.Load<GameObject>("Inventory/PlayerInventory"), canvasTransform).
                GetComponent<ItemBag>();

            currentContainer = Instantiate(Resources.Load<GameObject>("Inventory/Container"), canvasTransform).
                GetComponent<ItemContainer>();
        }

        private AudioClip GetSoundClip(Inventory.Sound sound)
        {
            switch (sound)
            {
                case Inventory.Sound.StartDrag:
                    return startDragSound;
                case Inventory.Sound.Drop:
                    return dropSound;
                case Inventory.Sound.Cancelled:
                    return cancelDragSound;
                default:
                    throw new UnityException("No sound clip for " + sound);
            }
        }

    }

    public static class Inventory
    {
        private static InventoryController controller;
        private static ItemBuilder itemBuilder;

        static Inventory()
        {
            itemBuilder = new ItemBuilder();
        }

        public static bool Ready => (controller != null);
        public static bool ContainerOpen => (controller.ContainerOpen);

        public static Sprite FoodInHand { set => controller.SetFoodInHand(value); }

        public static void SetController(InventoryController inventoryController)
        {
            controller = inventoryController;
        }

        public static void OpenCloseContainer(ItemList containerItems, string name)
        {
            if (ContainerOpen)
                controller.CloseContainer();
            else
                controller.OpenContainer(containerItems, name);
        }

        public static ItemSlot CreateSlot(Transform slotLocation)
        {
            return Object.Instantiate(Resources.Load<GameObject>("Inventory/Slot"), slotLocation)?.
                GetComponent<ItemSlot>();
        }

        public static Item CreateItem(ItemCode itemCode)
        {
            return itemBuilder.Build(itemCode);
        }

        public static Item CreateItem(ItemList.ItemInfo itemInfo)
        {
            var newItem = CreateItem(itemInfo.itemCode);
            return newItem;
        }

        public static void PlaceOnBag(ItemSlot itemSlot)
        {
            controller.PutInBag(itemSlot);
        }

        public static bool PlaceOnBag(List<ItemList.ItemInfo> itemlist)
        {
            if (!CanFitOnBag(itemlist))
                return false;
                
            controller.PutInBag(itemlist);

            return true;
        }

        public static void RemoveInBag(List<ItemList.ItemInfo> itemlist)
        {
            controller.RemoveInBag(itemlist);
        }

        public static void ShowBag()
        {
            controller.ShowBag();
        }

        public static void HideBag()
        {
            controller.HideBag();
        }

        public static bool CanFitOnBag(List<ItemList.ItemInfo> itemlist)
        {
            return controller.CanFitOnBag(itemlist);
        }

        public static void ResetBagItemActions()
        {
            controller.ResetBagActions();
        }

        public static void SetBagItemActions(UnityEngine.Events.UnityAction<ItemSlot> action)
        {
            controller.SetBagItemActions(action);
        }

        public static void SFX(Sound sound)
        {
            controller.PlaySFX(sound);
        }

        public enum Sound
        {
            StartDrag,
            Cancelled,
            Drop,
        }
    }
}