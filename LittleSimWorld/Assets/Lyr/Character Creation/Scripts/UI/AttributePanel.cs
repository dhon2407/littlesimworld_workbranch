using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreation {

	public class AttributePanel : SerializedMonoBehaviour {

		public UIAttributeType attributeType;
		public References references;

		int currentNumber;

		void Start() {
			references.HeaderText.text = attributeType.ToString();
			references.MoveLeft.onClick.AddListener(() => PerformAction(ArrowType.Left));
			references.MoveRight.onClick.AddListener(() => PerformAction(ArrowType.Right));
			// Update the UI
			PerformAction(0);

			// Register Action to Manager
			CharacterCreationManager.instance.OnGenderChanged += () => PerformAction(0);


		}

		void PerformAction(ArrowType arrowType) {
			int targetIndex = 0;
			CharacterCreationManager.instance.ChangeSprite(attributeType, arrowType, ref targetIndex);
			references.NumberText.text = (targetIndex + 1).ToString();
		}


		[System.Serializable]
		public class References {
			[TitleGroup("Buttons"), FitLabelWidth] public Button MoveLeft, MoveRight;

			[TitleGroup("Text"), FitLabelWidth] public TMPro.TextMeshProUGUI HeaderText, NumberText;
		}
	}
}