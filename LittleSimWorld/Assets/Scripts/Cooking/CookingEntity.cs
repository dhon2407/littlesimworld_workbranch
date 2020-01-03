using System.Collections.Generic;
using Cooking.Recipe;
using InventorySystem;
using UnityEngine;

using Stats = PlayerStats.Stats;
using static PlayerStats.Status;

namespace Cooking
{
    public abstract class CookingEntity : MonoBehaviour, IInteractable
    {
        protected static CookingEntity currentOpenCookingEntity;
        
        [SerializeField]
        protected float verticalDisplayOffset = 1480f;
        [SerializeField]
        protected float interactionRange = 0.2f;
        [SerializeField]
        protected float defaultEXP = 10f;
        
        [Space]
        [SerializeField]
        private Transform standArea = null;

        public abstract void Interact();

        public float InteractionRange => currentOpenCookingEntity.interactionRange;
        public Vector2 PlayerStandPosition => currentOpenCookingEntity.standArea.position;
        public Vector3 Position => currentOpenCookingEntity.transform.position;
        
        public static bool Open => currentOpenCookingEntity.onGoingAction;
        public static ItemCode LastCookedItem => currentOpenCookingEntity.GetLastCookedItem();
        public static ItemCode DefaultCookItem => currentOpenCookingEntity.DefaultItemToCook;

        public static string AutoActionText => currentOpenCookingEntity.AutoText;
        public static string ManualActionText => currentOpenCookingEntity.ManualText;
        public static string Text => currentOpenCookingEntity.ActionText;

        protected bool onGoingAction = false;
        protected abstract IEnumerator<float> StartAction(List<ItemList.ItemInfo> itemsToCook);
        protected abstract ItemCode GetLastCookedItem();
        protected abstract ItemCode DefaultItemToCook { get; }
        protected abstract float TimeToCook { get; set; }
        protected abstract bool ResumeAction { get; set; }
        protected abstract bool ActionCanceled { get; set; }
        protected abstract string AutoText { get; }
        protected abstract string ManualText { get; }
        protected abstract string ActionText { get; }
        protected abstract LastCookingData CookData { get; }

        public static void AutoAction()
        {
            currentOpenCookingEntity.TryAction(CookingHandler.AutoCookItem);
        }
        
        public static void ManualAction(List<ItemList.ItemInfo> itemsToCook)
        {
            currentOpenCookingEntity.TryAction(itemsToCook);
        }
        
        public static void Resume()
        {
            currentOpenCookingEntity.ResumeAction = true;
            if (currentOpenCookingEntity.ActionCanceled)
                currentOpenCookingEntity.TryAction(currentOpenCookingEntity.CookData.itemsToCook);
            else
                AutoAction();
        }
        
        public static void ResetAction()
        {
            currentOpenCookingEntity.ActionCanceled = false;
            currentOpenCookingEntity.CookData.Reset();
        }

        protected bool CanAct()
        {
            return (!onGoingAction &&
                    Stats.Status(Type.Energy).CurrentAmount > 5 &&
                    Stats.Status(Type.Health).CurrentAmount > 5);
        }

        protected bool InRange()
        {
            var distanceAway = Vector2.Distance(Position, GameLibOfMethods.player.transform.position);
            return distanceAway <= InteractionRange;
        }
        
        private void Update()
        {
            if (CookingHandler.Ongoing && !InRange())
                CookingHandler.ForceClose();
        }

        private void TryAction(List<ItemList.ItemInfo> itemsToCook)
        {
            if (CanAct())
            {
                CookingHandler.EnableCanvas = false;
                GameLibOfMethods.doingSomething = true;
                PlayerCommands.MoveTo(PlayerStandPosition, () => StartAction(itemsToCook).Start());    
            }
        }
        
        protected struct LastCookingData
        {
            public List<ItemList.ItemInfo> itemsToCook;
            public float timeLapse;
            public float exp;

            public void Reset()
            {
                itemsToCook = new List<ItemList.ItemInfo>();
                timeLapse = 0;
                exp = 10; //minimum default
            }
        }
        
        #region Cheat Codes

        public static float CheatEXP
        {
            set => currentOpenCookingEntity.defaultEXP = value;
        }

        public static float CheatTimeCompletion
        {
            set => currentOpenCookingEntity.TimeToCook = value;
        }

        #endregion
        
    }
}