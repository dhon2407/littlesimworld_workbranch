﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UI.Cooking;
using UnityEngine;
using System.Linq;

namespace UI.Cooking {
	public class UI_IngredientPanel : MonoBehaviour {

		public UI_IngredientPopupSlot PopupSlotPrefab;

		[Space, Header("Spawn Animation Settings")]
		public float OffsetY = 0.3f;
		public float TotalExpandTime = 0.5f;
		public float TimeBetweenNewSpawns = 0.1f;


		public static UI_IngredientPanel instance;

		// Slots that need to be destroyed when this disappears
		List<UI_IngredientPopupSlot> spawnedSlots = new List<UI_IngredientPopupSlot>();


		void Awake() {
			instance = this;
		}

		public static void SpawnFor(Transform target) => instance.SpawnOnTarget(target);
		public static void Despawn() => instance.Disappear();

		void Disappear() {
			ClearList();
			StopAllCoroutines();
			gameObject.SetActive(false);
		}
		void ClearList() {
			foreach (var item in spawnedSlots) {
				Destroy(item.gameObject);
			}
			spawnedSlots.Clear();
		}

		void OnDisable() {
			ClearList();
		}

		void SpawnOnTarget(Transform target) {
			
			var targetPos = target.position + Vector3.up * OffsetY;
			if (transform.position == targetPos) { ClearList(); }
			transform.position = targetPos;
			transform.localScale = Vector3.zero;

			this.gameObject.SetActive(true);
			StopAllCoroutines();
			StartCoroutine(StartExpanding());
			StartCoroutine(PopupItems(target.GetComponent<UI_ManualCookingSlot>()));
		}

		IEnumerator StartExpanding() {
			float t = 0;
			while (t < TotalExpandTime) {
				t += Time.deltaTime;
				transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / TotalExpandTime);
				yield return null;
			}
		}

		IEnumerator PopupItems(UI_ManualCookingSlot parent) {
			// var items = Inventory.GetItems(item => item.isIngredient);
			// if (items.Count == 0) {
			// ...
			ClearList();

			bool spawnedAtleastOne = false;

			foreach (var itemSlot in AtommInventory.inventory) {
				Item item = ItemsManager.GetItemWithName(itemSlot.itemName);

				if (!item || !item.Description.ToLower().Contains("ingridient")) { continue; }
				var popup = Instantiate(PopupSlotPrefab, transform);

				popup.SetItem(item);
				popup.SetParent(parent);

				spawnedSlots.Add(popup);
				spawnedAtleastOne = true;
				yield return new WaitForSeconds(TimeBetweenNewSpawns);
			}

			if (!spawnedAtleastOne) { Instantiate(PopupSlotPrefab, transform); }
			yield return null;
		}


	}
}