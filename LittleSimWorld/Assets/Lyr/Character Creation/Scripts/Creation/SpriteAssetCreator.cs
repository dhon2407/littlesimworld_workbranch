#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace CharacterData {
	[CreateAssetMenu]
	public class SpriteAssetCreator : Utilities.ScriptableObjectCreator<CharacterSpriteSet> {

		[ShowInInspector]
		CharacterPart SetType { get; set; }

		protected override string SavePath => "Assets/ScriptableObjects/Sprite Assets/" + SetType.ToString();

		protected override void AdditionalInitialization(CharacterSpriteSet obj) { }
		protected override bool AdditionalValidation() {
			if (SetType == CharacterPart.None) {
				Debug.Log("Select a Set Type from above");
				return false;
			}
			return true;
		}
		protected override void AdditionalReset() { }// => SetType = CharacterPart.None;


		
		[Header("Mass Import")]
		[PropertyOrder(999)] public string Prefix;
		[PropertyOrder(1000)] public int NextIndex;
		[PropertyOrder(1001)] public List<Sprite> SpriteCollection = new List<Sprite>();

		[Button]
		[PropertyOrder(1002)]
		void MassSpawn() {

			int SetAmount = SpriteCollection.Count / 4;
			for (int i = 0; i < SetAmount; i += 4) {
				item.Bot = SpriteCollection[i + 0];
				item.Top = SpriteCollection[i + 1];
				item.Right = SpriteCollection[i + 2];
				item.Left = SpriteCollection[i + 3];

				SetType = GetType(i);
				Name = GetName(i);

				CreateAsset();
			}

			SpriteCollection.Clear();
		}

		string GetName(int i) {
			string s = "";
			if (i == 0) { s += ""; }
			if (i == 1) { s += ""; }
			if (i == 2) { s += ""; }
			if (i == 3) { s += ""; }
			if (i == 4) { s += ""; }

			return s;
		}

		CharacterPart GetType(int i) {
			if (i == 0) { return CharacterPart.Head; }
			if (i == 1) { return CharacterPart.Body; }
			if (i == 2) { return CharacterPart.Hands; }
			return CharacterPart.None;
		}
		
	}
}
#endif
