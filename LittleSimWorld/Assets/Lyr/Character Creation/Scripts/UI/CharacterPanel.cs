using CharacterData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreation {
	public class CharacterPanel : MonoBehaviour {

		public Button LeftButton, RightButton, RandomizeButton;

		[Space, Header("Selection Panel Settings")]
		public RectTransform SelectionPanel;
		Vector3 originalPanelPos;
		public float Offset = 81;


		void Awake() {
			LeftButton.onClick.AddListener(ChangeGender);
			RightButton.onClick.AddListener(ChangeGender);
			RandomizeButton.onClick.AddListener(Randomize);

			originalPanelPos = SelectionPanel.transform.localPosition;
		}

		void Randomize() {
			// We don't use this
			int _unused = 2;

			foreach (var enumValue in System.Enum.GetValues(typeof(UIAttributeType))) {
				for (int i = 0; i < 30; i++) {
					int arrowInt = Random.Range(-1, 2);
					CharacterCreationManager.instance.ChangeSprite((UIAttributeType) enumValue, (ArrowType) arrowInt, ref _unused);
				}
			}
			CharacterCreationManager.instance.ChangeGender((ArrowType) Random.Range(-1, 0));
		}

		void ChangeGender() {
			CharacterCreationManager.instance.ChangeGender(ArrowType.Left);
			SetSelectionPanelPosition();
		}

		[Button]
		void SetSelectionPanelPosition() {
			var targetPos = originalPanelPos;
			if (CharacterCreationManager.instance.gender == Gender.Female) { targetPos += Offset * Vector3.right; }
			SelectionPanel.transform.localPosition = targetPos;

		}
	}
}