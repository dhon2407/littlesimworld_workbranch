using System.Collections.Generic;
using Cooking.Recipe;
using InventorySystem;
using UnityEngine;
using static PlayerStats.Status;
using PlayerStats;

namespace Cooking
{
    [AddComponentMenu("Cooking/Stove")]
    public class CookingStove : MonoBehaviour, IInteractable
    {
        private static CookingStove instance;
        private static ItemCode lastCookItem = ItemCode.NONE;
        private static bool cookingCanceled;
        private static bool resumeCooking;

        [SerializeField]
        private float verticalDisplayOffset = 1480f;
        [SerializeField]
        private float interactionRange = 0.2f;
        [SerializeField]
        private float cookingEXP = 10f;

        [Space]
        [SerializeField]
        private GameObject fryingPan = null;

        [Space]
        [SerializeField]
        private Transform standArea = null;

        private bool isCooking;
        private LastCookingData cookData;
        private float minimumInteractionRange;
        private float timeToCook = 10f;

        public float InteractionRange => interactionRange;
        public Vector2 PlayerStandPosition => standArea.position;
        public static ItemCode LastCookedItem => lastCookItem;
        public static bool Open => instance.isCooking;

        private Vector3 StovePosition => transform.position;

        public void Interact()
        {
            if (!CanCook()) return;

            var displayPosition = StovePosition;
            displayPosition.y += verticalDisplayOffset;
            CookingHandler.ToggleView(Camera.main.WorldToViewportPoint(displayPosition), !cookingCanceled);

            interactionRange = Mathf.Clamp(Vector2.Distance(StovePosition, GameLibOfMethods.player.transform.position),
                minimumInteractionRange, float.MaxValue);
        }

        public static void ManualCook(List<ItemList.ItemInfo> itemsToCook)
        {
            instance.TryCook(itemsToCook);
        }

        public static void AutoCook()
        {
            instance.TryCook(CookingHandler.AutoCookItem);
        }

        private void TryCook(List<ItemList.ItemInfo> itemsToCook)
        {
            if (CanCook())
            {
                CookingHandler.EnableCanvas = false;
                GameLibOfMethods.doingSomething = true;
                PlayerCommands.MoveTo(standArea.position, () => StartCooking(itemsToCook).Start());
            }
        }

        private void Awake()
        {
            instance = this;
            minimumInteractionRange = interactionRange;
        }

        private void Update()
        {
            if (CookingHandler.Ongoing && !InRange())
                CookingHandler.ForceClose();
        }

        private bool InRange()
        {
            var distanceAway = Vector2.Distance(StovePosition, GameLibOfMethods.player.transform.position);
            return distanceAway <= interactionRange;
        }

        private bool CanCook()
        {
            return (!isCooking &&
                    Stats.Status(Type.Energy).CurrentAmount > 5 &&
                    Stats.Status(Type.Health).CurrentAmount > 5);
        }

		private IEnumerator<float> StartCooking(List<ItemList.ItemInfo> itemsToCook)
        {
            cookData.itemsToCook = itemsToCook;

			isCooking = true;
            fryingPan.SetActive(false);
            
            CookingHandler.ForceClose();

            GameLibOfMethods.canInteract = false;
			GameLibOfMethods.cantMove = true;
			GameLibOfMethods.Walking = true;
			SpriteControler.Instance.FaceUP();

			GameLibOfMethods.animator.SetBool("Cooking", true);
			UIManager.Instance.ActionText.text = "Cooking";

			float timeLapse = cookingCanceled ? cookData.timeLapse : 0;
            cookingEXP = cookingCanceled ? cookData.exp : 10f;

            while (timeLapse < timeToCook)
			{
				timeLapse += Time.deltaTime;
				GameLibOfMethods.progress = timeLapse / timeToCook;
				if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Escape))
				{
                    resumeCooking = false;
                    cookingCanceled = true;
                    cookData.exp = cookingEXP;
                    cookData.timeLapse = timeLapse;
                    break;
				}

                if (Stats.Status(Type.Energy).CurrentAmount <= 0 ||
                    Stats.Status(Type.Health).CurrentAmount <= 0)
                {
                    break;
                }

				yield return 0f;
			}

			GameTime.Clock.ChangeSpeed(5);

			fryingPan.SetActive(true);
			yield return 0f;

			isCooking = false;

			GameLibOfMethods.progress = 0;
            PlayerAnimationHelper.ResetPlayer();

            if (Stats.Status(Type.Energy).CurrentAmount <= 0 || Stats.Status(Type.Health).CurrentAmount <= 0)
			{
				yield return 0f;
				GameLibOfMethods.animator.SetBool("PassOut", true);
			}
			else if (resumeCooking || !cookingCanceled)
            {
				Stats.AddXP(Skill.Type.Cooking, cookingEXP);

				yield return 0f;

                CookingHandler.AddCookedRecipes(itemsToCook);
                Inventory.PlaceOnBag(itemsToCook);

                if (itemsToCook.Count > 0)
                    lastCookItem = itemsToCook[itemsToCook.Count - 1].itemCode;

				yield return 0f;

                cookingCanceled = false;
                resumeCooking = false;
                cookData.Reset();
            }
		}

        public static void ResumeCook()
        {
            resumeCooking = true;
            if (cookingCanceled)
                instance.TryCook(instance.cookData.itemsToCook);
            else
                AutoCook();
        }

        public static void ResetStove()
        {
            cookingCanceled = false;
            instance.cookData.Reset();
        }

        #region MyRegion

        public static float CheatEXP
        {
            set => instance.cookingEXP = value;
        }

        public static float CheatTimeCompletion
        {
            set => instance.timeToCook = value;
        }

        #endregion

        private struct LastCookingData
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
    }
}
