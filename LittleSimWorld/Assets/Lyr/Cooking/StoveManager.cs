using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cooking {

	public enum MenuState { Open, Closed }
	public enum MenuSide { Left, Right }
	public delegate void UIAction();

	[DefaultExecutionOrder(-1)]
	public class StoveManager : MonoBehaviour {

		[SerializeField] GameObject CookingBase;
		[SerializeField] UI_ManualCookingPanel ManualCookingPanel;

		[Space]
		[SerializeField] Button AutoCookButton;
		[SerializeField] Button ManualCookButton;
		[SerializeField] Button CancelCookButton;

		public static StoveManager instance;

		[Header("Menu Animation Settings")]
		[SerializeField] float ExpandSpeed = 10;
		[SerializeField] float XShift = 1;
		[SerializeField] float ShiftSpeed = 10;

		GraphicRaycaster raycaster;
		Vector3 originalCookingBasePos;

		public event UIAction OnClosing;
		public event UIAction OnClose;

		void Awake() {
			instance = this;
			raycaster = GetComponent<GraphicRaycaster>();
			originalCookingBasePos = CookingBase.transform.position;

			CookingBase.transform.localScale = Vector3.zero;
			CookingBase.SetActive(false);

			AutoCookButton.onClick.AddListener(AutoCook);
			ManualCookButton.onClick.AddListener(ManualCook);
			CancelCookButton.onClick.AddListener(CloseMenu);

			//OnClosing += () => CookingBase.transform.position = originalCookingBasePos;
			//OnClosing += () => CookingBase.SetActive(false);
		}

		public void ToggleMenu(MenuState? targetState = null) {
			if (targetState != null) { CurrentMenuState = targetState.Value; }
			else {
				if (CurrentMenuState == MenuState.Closed) { CurrentMenuState = MenuState.Open; }
				else { CurrentMenuState = MenuState.Closed; }
			}
			StopAllCoroutines();
			StartCoroutine(AnimateMenu(CurrentMenuState));
		}

		void AutoCook() {
			_CookingStove.instance.Cook(null);
			CloseMenu();
		}

		void ManualCook() {
			var state = ManualCookingPanel.ChangePanelState();
			StopAllCoroutines();
			StartCoroutine(AnimatePosition(state));
		}

		public void CloseMenu() => ToggleMenu(MenuState.Closed);


		MenuState CurrentMenuState = MenuState.Closed;
		IEnumerator AnimateMenu(MenuState targetMenuState) {
			Vector3 targetSize = (targetMenuState == MenuState.Open) ? Vector3.one : Vector3.zero;

			// Disable raycasting until the menu fully expands
			raycaster.enabled = false;

			if (targetMenuState == MenuState.Open) { CookingBase.SetActive(true); }
			else { OnClosing(); }

			// Start expanding the Menu

			Vector3 currentSize = CookingBase.transform.localScale;
			while (currentSize != targetSize) {
				float _spd = ExpandSpeed * Time.deltaTime;
				currentSize = Vector3.MoveTowards(currentSize, targetSize, _spd);

				CookingBase.transform.localScale = currentSize;

				yield return null;
			}

			if (targetMenuState == MenuState.Closed) {
				OnClose();
				CookingBase.transform.position = originalCookingBasePos;
				CookingBase.SetActive(false);
			}
			else {
				raycaster.enabled = true;
			}
		}

		IEnumerator AnimatePosition(MenuState OtherPanelMenuState) {
			Vector3 targetPos = OtherPanelMenuState == MenuState.Open ? originalCookingBasePos + Vector3.left * XShift : originalCookingBasePos;
			while (CookingBase.transform.position != targetPos) {
				float _spd = ShiftSpeed * Time.deltaTime;
				CookingBase.transform.position = Vector3.MoveTowards(CookingBase.transform.position, targetPos, _spd);
				yield return null;
			}
		}

		void OnDrawGizmosSelected() {
			Gizmos.DrawWireSphere(CookingBase.transform.position, 0.2f);
			Gizmos.DrawWireSphere(CookingBase.transform.position + Vector3.left * XShift, 0.2f);
			Gizmos.DrawWireSphere(CookingBase.transform.position + Vector3.left * XShift, 0.4f);
		}
	}
}