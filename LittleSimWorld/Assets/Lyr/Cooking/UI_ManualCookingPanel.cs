using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cooking {

	public class UI_ManualCookingPanel : MonoBehaviour {

		public GameObject BasePanel;
		public Button ConfirmButton;
		public Button ClearAllButton;

		[Space, Header("UI Slots")]
		public UI_ManualCookingSlot Slot1;
		public UI_ManualCookingSlot Slot2;
		public UI_ManualCookingSlot Slot3;

		[Header("Expand Options")]
		public float ExpandSpeed;

		MenuState CurrentMenuState = MenuState.Closed;
		void Awake() {
			FastClosePanel();

			StoveManager.instance.OnClosing += FastClosePanel;
			StoveManager.instance.OnClose += FastClosePanel;

			ConfirmButton.onClick.AddListener(ConfirmCooking);
			ClearAllButton.onClick.AddListener(ClearSlots);
		}

		void EvaluateSlotState() {
			Slot1.GreyedOut(true);
			Slot2.GreyedOut(Slot2.isAvailableForPlayer);
			Slot3.GreyedOut(Slot3.isAvailableForPlayer);
		}
		public void ClearSlots() {
			Slot1.ClearSlot();
			Slot2.ClearSlot();
			Slot3.ClearSlot();
		}

		void FastClosePanel() {
			CurrentMenuState = MenuState.Closed;
			BasePanel.transform.localScale = Vector3.zero;
			BasePanel.SetActive(false);
		}

		void ConfirmCooking()
        {
            var ingredients = GetIngredients();

            if (ingredients.Count > 0)
            {
                _CookingStove.instance.Cook(GetIngredients());
                ClearSlots();
            }
            else
            {
                GameLibOfMethods.CreateFloatingText("Stove is empty.", 1.5f);
            }
		}

		public MenuState ChangePanelState(MenuState? TargetMenuState = null) {
			if (TargetMenuState == null)
            {
				if (CurrentMenuState == MenuState.Closed)
                    CurrentMenuState = MenuState.Open;
				else
                    CurrentMenuState = MenuState.Closed;
			}
			else
            {
                CurrentMenuState = TargetMenuState.Value;
            }

            if (CurrentMenuState == MenuState.Open)
            {
                EvaluateSlotState();
                InventorySystem.Inventory.SetBagItemActions(StoveManager.instance.PlaceItemOnSlot);
            }
            else
            {
                InventorySystem.Inventory.SetBagItemActions(null);
            }

            ClearSlots();

            StartCoroutine(AnimateMenu(CurrentMenuState));
			UI_IngredientPanel.Despawn();
			return CurrentMenuState;
		}

		List<InventorySystem.ItemCode> GetIngredients() {

			var ingredients = new List<InventorySystem.ItemCode>();

            if (!Slot1.Empty)
                ingredients.Add(Slot1.ItemCode);
            if (!Slot2.Empty)
                ingredients.Add(Slot2.ItemCode);
            if (!Slot3.Empty)
                ingredients.Add(Slot3.ItemCode);

            return ingredients;
		}


		IEnumerator AnimateMenu(MenuState targetMenuState) {

			Vector3 targetSize = (targetMenuState == MenuState.Open) ? Vector3.one : Vector3.zero;

			if (targetMenuState == MenuState.Open) {
				BasePanel.SetActive(true);
			}

			// Start expanding
			Vector3 currentSize = BasePanel.transform.localScale;
			while (currentSize != targetSize) {
				float _spd = ExpandSpeed * Time.deltaTime;
				currentSize = Vector3.MoveTowards(currentSize, targetSize, _spd);

				BasePanel.transform.localScale = currentSize;
				yield return null;
			}

			if (targetMenuState == MenuState.Closed) { BasePanel.SetActive(false); }
		}

	}

}