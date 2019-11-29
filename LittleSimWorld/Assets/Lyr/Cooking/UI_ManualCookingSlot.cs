using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Cooking {
	public class UI_ManualCookingSlot : MonoBehaviour, IPointerDownHandler {
		public Item holdingItem;
		Image SlotImage;

		public int RequiredCookingLevel;

		[HideInInspector] public bool isAvailableForPlayer;

		void Awake() {
			SlotImage = transform.GetChild(0).GetComponent<Image>();
		}

		void Update() {
			CheckAvailability();
		}

		void CheckAvailability() {
			if (isAvailableForPlayer) { return; }

			isAvailableForPlayer = PlayerStatsManager.Instance.playerSkills[SkillType.Cooking].Level >= RequiredCookingLevel;
			// To be swapped with sprite
			SlotImage.color = isAvailableForPlayer ? Color.white : Color.grey;
		}

		public void SetItem(Item item) {
			Sprite spr = item != null ? ItemsManager.GetSpriteOfItem(item) : null;
			if (!SlotImage) { Awake(); }
			SlotImage.sprite = spr;
			holdingItem = item;
		}

		public void OnPointerDown(PointerEventData eventData) {
			if (!isAvailableForPlayer) { return; }

			UI_IngredientPanel.SpawnFor(transform);
		}
	}
}