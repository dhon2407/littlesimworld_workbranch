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

		void Update() {
			EvaluateSlotState();	
		}

		void EvaluateSlotState() {
			Slot1.gameObject.SetActive(true);
			Slot2.gameObject.SetActive(Slot1.isAvailableForPlayer);
			Slot3.gameObject.SetActive(Slot2.isAvailableForPlayer);
		}
		void ClearSlots() {
			Slot1.SetItem(null);
			Slot2.SetItem(null);
			Slot3.SetItem(null);
		}

		void FastClosePanel() {
			CurrentMenuState = MenuState.Closed;
			BasePanel.transform.localScale = Vector3.zero;
			BasePanel.SetActive(false);
		}

		void ConfirmCooking() {
			_CookingStove.instance.Cook(GetIngredients());
			ClearSlots();
		}

		public MenuState ChangePanelState(MenuState? TargetMenuState = null) {
			if (TargetMenuState == null) {
				if (CurrentMenuState == MenuState.Closed) { CurrentMenuState = MenuState.Open; }
				else { CurrentMenuState = MenuState.Closed; }
			}
			else { CurrentMenuState = TargetMenuState.Value; }
			StartCoroutine(AnimateMenu(CurrentMenuState));
			UI_IngredientPanel.Despawn();
			return CurrentMenuState;
		}

		List<Item> GetIngredients() {

			List<Item> ingredients = new List<Item>(3);

			if (Slot1.holdingItem) { ingredients.Add(Slot1.holdingItem); }
			if (Slot2.holdingItem) { ingredients.Add(Slot2.holdingItem); }
			if (Slot3.holdingItem) { ingredients.Add(Slot3.holdingItem); }

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