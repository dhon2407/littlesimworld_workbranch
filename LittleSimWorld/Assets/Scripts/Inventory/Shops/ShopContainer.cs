using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace InventorySystem
{
    public class ShopContainer : MonoBehaviour
    {
        public List<Item> SellingItems;
        public GameObject ShopOptionsUI;
        public Transform playerWorkPlace;
        public string WorkingAnimationName;
        public Collider2D ShopZone;
        public Outline shopOutline;
        public PlayerStatsManager playerStatsManager;
        
        [Header("SFX")]
        [SerializeField]
        private AudioClip open;
        [SerializeField]
        private AudioClip close;
        
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            shopOutline = gameObject.GetComponent<Outline>();
        }

        private void Update()
        {
            //TODO
            //if (ShopOptionsUI)
            //{
            //    if (GameLibOfMethods.doingSomething || !GameLibOfMethods.canInteract || GameLibOfMethods.cantMove)
            //    {
            //        ShopOptionsUI.SetActive(false);
            //    }
            //}
        }

    }
}