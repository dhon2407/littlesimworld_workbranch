using UnityEngine;

namespace InventorySystem
{
    public class ShopsManager : MonoBehaviour
    {
        private static ShopsManager instance;

        private static ShoppingWindow shoppingWindow = null;
        private static ShopList currentShoplist = null;

        private static UpgradesShop upgradesShop = null;
        private static UpgradeList currentUpgradelist = null;

        private bool shopOpen;
        private bool upgradeShopOpen;

        public bool ShopOpen => shopOpen;
        public bool UpgradeShopOpen => upgradeShopOpen;

        [SerializeField]
        private Transform canvasTransform = null;
        [SerializeField]
        private AudioSource audioSource = null;

        [Header("SFX")]
        [SerializeField]
        private AudioClip openShop = null;
        [SerializeField]
        private AudioClip closeShop = null;
        

        public void OpenShop(ShopList shoplist, string name)
        {
            currentShoplist = shoplist;
            shoppingWindow.SetName(name);
            shoppingWindow.Open(shoplist);

            Inventory.ShowBag();

            shopOpen = true;

            PlayOpenSFX();
        }

        public void OpenUpgradesShop(UpgradeList upgradeList, string name)
        {
            currentUpgradelist = upgradeList;
            upgradesShop.SetName(name);
            upgradesShop.Open(upgradeList);

            Inventory.ShowBag();

            upgradeShopOpen = true;

            PlayOpenSFX();
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            Shop.SetManager(this);
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (shopOpen)
            {
                var shopList = GameLibOfMethods.CheckInteractable()?.GetComponent<ShopList>();
                if (shopList == null || !shopList.Equals(currentShoplist))
                    CloseShop();
            }

            if (upgradeShopOpen)
            {
                var upgradeList = GameLibOfMethods.CheckInteractable()?.GetComponent<UpgradeList>();
                if (upgradeList == null || !upgradeList.Equals(currentUpgradelist))
                    CloseUpgradeShop();
            }
        }

        public void CloseShop()
        {
            shoppingWindow.Close();
            currentShoplist = null;

            Inventory.HideBag();

            shopOpen = false;

            PlayCloseSFX();
        }

        public void CloseUpgradeShop()
        {
            upgradesShop.Close();
            currentUpgradelist = null;

            Inventory.HideBag();

            upgradeShopOpen = false;

            PlayCloseSFX();
        }

        private void Initialize()
        {
            shoppingWindow = Instantiate(Resources.Load<GameObject>("Inventory/Shops/ShoppingWindow"), canvasTransform).
                GetComponent<ShoppingWindow>();

            upgradesShop = Instantiate(Resources.Load<GameObject>("Inventory/Shops/UpgradeShop"), canvasTransform).
                GetComponent<UpgradesShop>();
        }

        private void PlayOpenSFX()
        {
            if (audioSource != null && openShop != null)
            {
                audioSource.clip = openShop;
                audioSource.Play();
            }
        }

        private void PlayCloseSFX()
        {
            if (audioSource != null && closeShop != null)
            {
                audioSource.clip = closeShop;
                audioSource.Play();
            }
        }
    }



    public static class Shop
    {
        private static ShopsManager manager;

        public static bool Ready => (manager != null);
        public static bool IsShopOpen => manager.ShopOpen;
        public static bool IsUpgradeShopOpen => manager.UpgradeShopOpen;

        public static void SetManager(ShopsManager shopsManager)
        {
            manager = shopsManager;
        }

        public static void OpenCloseShop(ShopList shoplist, string name)
        {
            if (IsShopOpen)
                manager.CloseShop();
            else
                manager.OpenShop(shoplist, name);
        }

        public static void OpenCloseUpgradeShop(UpgradeList upgradeList, string name)
        {
            if (IsUpgradeShopOpen)
                manager.CloseUpgradeShop();
            else
                manager.OpenUpgradesShop(upgradeList, name);
        }

        public static void CloseUpgradeShop()
        {
            if (IsUpgradeShopOpen)
                manager.CloseUpgradeShop();
        }

        public static ShopItem CreateShopItem(Transform holder)
        {
            return Object.Instantiate(Resources.Load<GameObject>("Inventory/Shops/ShopSlot"), holder)?.
                GetComponent<ShopItem>();
        }

        public static BasketItem CreateBasketItem(Transform holder)
        {
            return Object.Instantiate(Resources.Load<GameObject>("Inventory/Shops/BasketSlot"), holder)?.
                GetComponent<BasketItem>();
        }

        public static BasketItem CreateUpgradeItem(Transform holder)
        {
            return Object.Instantiate(Resources.Load<GameObject>("Inventory/Shops/UpgradeSlot"), holder)?.
                GetComponent<BasketItem>();
        }
    }
}