using System.Collections.Generic;
using CharacterData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.RandomNPC {

[HideReferenceObjectPicker]
public class VisualsHelper {

		const float minFacePosition = -0.058f;
		const float maxFacePosition = -0.018f;

		Dictionary<CharacterPart, CharacterSpriteSet> SpriteSets;
		public Dictionary<CharacterPart, SpriteRenderer> bodyParts;

		public SpriteRenderer Hand_L;
		public SpriteRenderer Hand_R;

		public void AssignRandomSet() {
			SpriteSets = SpriteSets.InitializeDefaultValues();
			SpriteSets.Remove(CharacterPart.None);

			// Randomize gender and looks.
			int tone = Random.Range(0, 5);
			int Gender = Random.Range(0, 2);
			var keys = bodyParts.Keys;
			foreach (var key in keys) { SpriteSets[key] = CharacterClothingManager.instance.GetRandom(tone, (Gender) Gender, key); }

			// Randomize face positioning.
			float faceT = Random.Range(0f, 1f);
			var facePos = bodyParts[CharacterPart.Face].transform.localPosition;
			facePos.y = Mathf.Lerp(minFacePosition, maxFacePosition, faceT);
			bodyParts[CharacterPart.Face].transform.localPosition = facePos;
		}

		public void FaceUP() => UpdateVisuals(CharacterOrientation.Top);
		public void FaceDOWN() => UpdateVisuals(CharacterOrientation.Bot);
		public void FaceRIGHT() => UpdateVisuals(CharacterOrientation.Right);
		public void FaceLEFT() => UpdateVisuals(CharacterOrientation.Left);


		void UpdateVisuals(CharacterOrientation orientation) {
			var keys = bodyParts.Keys;

			foreach (var key in keys) {
				if (bodyParts[key] == null || SpriteSets[key] == null) { continue; }

				if (key != CharacterPart.Hands) {
					bodyParts[key].sprite = SpriteSets[key].Get(orientation);
				}
				else {
					Hand_L.sprite = SpriteSets[key].Get(orientation);
					Hand_R.sprite = SpriteSets[key].Get(orientation);
				}
			}
		}

	}
}