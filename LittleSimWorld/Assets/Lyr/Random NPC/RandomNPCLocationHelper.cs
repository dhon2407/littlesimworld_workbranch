namespace Characters.RandomNPC {
	using System.Collections;
	using System.Collections.Generic;
	using Sirenix.OdinInspector;
	using UnityEngine;

	[HideReferenceObjectPicker]
	public class RandomNPCLocationHelper {

		[HideReferenceObjectPicker, ListDrawerSettings(AddCopiesLastElement = true)] public List<NPCLocation> SpawnLocations;
		[HideReferenceObjectPicker, ListDrawerSettings(AddCopiesLastElement = true)] public List<NPCLocation> GoToLocations;

		[LabelFromMethod("ToString"), HideReferenceObjectPicker]
		public class NPCLocation {
			public string Name = "Location";
			public Vector2 Position;

			public static implicit operator Vector2(NPCLocation npc) => npc.Position;
			public static implicit operator Vector3(NPCLocation npc) => npc.Position;

			public override string ToString() => Name;
		}
	}
}
