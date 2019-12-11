using UnityEngine;

namespace InventorySystem
{
    public class ShopsManager : MonoBehaviour
    {
        private static ShopsManager instance;

        private static ShoppingWindow shoppingWindow = null;
        private static ShopList currentShoplist = null;

        private bool shopOpen;

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
                    CloseCurrentShop();
            }
        }

        private void CloseCurrentShop()
        {
            shoppingWindow.Close();
            currentShoplist = null;

            Inventory.HideBag();

            shopOpen = false;
        }

        private void Initialize()
        {
            shoppingWindow = Instantiate(Resources.Load<GameObject>("Inventory/Shops/ShoppingWindow"), canvasTransform).
                GetComponent<ShoppingWindow>();
        }
    }



    public static class Shop
    {
        private static ShopsManager manager;

        public static bool Ready => (manager != null);

        public static void SetManager(ShopsManager shopsManager)
        {
            manager = shopsManager;
        }

        public static void Open(ShopList shoplist, string name)
        {
            manager.OpenShop(shoplist, name);
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
    }
}