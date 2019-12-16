using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Cooking {
	public class UI_IngredientPopupSlot : MonoBehaviour, IPointerDownHandler {
		public Image ItemImage;

		[Header("Popup Settings")]
		public float TotalTimeToGrow;
		public ParticleSystem particleToSpawnOnFail;
		public float particleSpawnSize = 0.2f;

		Action OnClick;

		public void SetParent(UI_ManualCookingSlot slot) {
			OnClick += () => UI_IngredientPanel.Despawn();
		}

		void Awake() => StartCoroutine(PopUp());

		IEnumerator PopUp() {
			transform.localScale = Vector3.zero;

			Vector3 TargetSize = Vector3.one;
			Vector3 CurrentSize = Vector3.zero;

			float t = 0;
			while (CurrentSize != TargetSize) {
				t += Time.deltaTime;
				float T = t / TotalTimeToGrow;
				CurrentSize = Vector3.Lerp(Vector3.zero, Vector3.one, T);
				transform.localScale = CurrentSize;
				yield return 0f;
			}

			StartCoroutine(PoofWithStyle());
		}

		IEnumerator PoofWithStyle() {

			float t = 0;
			while (t < 0.1f) {
				t += Time.deltaTime;
				float T = t / 0.1f;
				transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, T);
				yield return 0f;
			}
			t = 0;
			while (t < 0.2f) {
				t += Time.deltaTime;
				float T = t / 0.2f;
				transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, T);
				yield return 0f;
			}

			if (particleToSpawnOnFail) {
				var particle = Instantiate(particleToSpawnOnFail, transform.position, Quaternion.identity);
				particle.transform.localScale = Vector3.one * particleSpawnSize;
			}
			Destroy(this.gameObject);
		}

		public void OnPointerDown(PointerEventData eventData) {
			OnClick();

		}

	}
}