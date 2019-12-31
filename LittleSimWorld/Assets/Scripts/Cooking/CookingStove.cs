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

        public float InteractionRange => interactionRange;
        public Vector3 PlayerStandPosition => standArea.position;
        public static ItemCode LastCookedItem => lastCookItem;
        private Vector3 stovePosition => transform.position;

        public void Interact()
        {
            if (!CanCook()) return;

            var displayPosition = stovePosition;
            displayPosition.y += verticalDisplayOffset;
            CookingHandler.ToggleView(Camera.main.WorldToViewportPoint(displayPosition));
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
        }

        private void Update()
        {
            if (CookingHandler.Ongoing && !InRange())
                CookingHandler.ForceClose();
        }

        private bool InRange()
        {
            return Vector2.Distance(stovePosition, GameLibOfMethods.player.transform.position) < interactionRange;
        }

        private bool CanCook()
        {
            return (!isCooking &&
                    Stats.Status(Type.Energy).CurrentAmount > 5 &&
                    Stats.Status(Type.Health).CurrentAmount > 5);
        }

		private IEnumerator<float> StartCooking(List<ItemList.ItemInfo> itemsToCook)
		{
			isCooking = true;
            fryingPan.SetActive(false);
            
            CookingHandler.ForceClose();

            GameLibOfMethods.canInteract = false;
			GameLibOfMethods.cantMove = true;
			GameLibOfMethods.Walking = true;
			SpriteControler.Instance.FaceUP();

			GameLibOfMethods.animator.SetBool("Cooking", true);
			UIManager.Instance.ActionText.text = "Cooking";
			bool cookingCanceled = false;

			float timeLapse = 0;
            float TimeToCook = 10f;

            while (timeLapse < TimeToCook)
			{
				timeLapse += Time.deltaTime;
				GameLibOfMethods.progress = timeLapse / TimeToCook;
				if (Input.GetKeyUp(KeyCode.E))
				{
					cookingCanceled = true;
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

			if (Stats.Status(Type.Energy).CurrentAmount <= 0 || Stats.Status(Type.Health).CurrentAmount <= 0)
			{
				PlayerAnimationHelper.ResetPlayer();
				yield return 0f;
				GameLibOfMethods.animator.SetBool("PassOut", true);
			}
			else if (cookingCanceled)
			{
				PlayerAnimationHelper.ResetPlayer();
				yield return 0f;
			}
			else
			{
				Stats.AddXP(Skill.Type.Cooking, cookingEXP);

				yield return 0f;

                CookingHandler.AddCookedRecipes(itemsToCook);
                Inventory.PlaceOnBag(itemsToCook);

                if (itemsToCook.Count > 0)
                    lastCookItem = itemsToCook[itemsToCook.Count - 1].itemCode;

                PlayerAnimationHelper.ResetPlayer();
				yield return 0f;
			}
		}
	}
}
