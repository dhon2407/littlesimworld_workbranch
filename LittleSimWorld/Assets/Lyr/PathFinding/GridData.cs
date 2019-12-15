using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using UnityEngine;
using System.IO;

namespace PathFinding {
	public class GridData : SerializedScriptableObject {
		[System.NonSerialized] public Node[,] nodeGrid;
		[System.NonSerialized] public bool attemptedLoad;

		string filePath => Application.dataPath + "/GridData/" + name + ".GRID_DATA";

		[Button] public void Save() => SaveData();

		[Button] public void Load() => LoadData();

		public void SaveData() {
#if UNITY_EDITOR
			if (nodeGrid == null) { Debug.Log("ERROR: Node list empty"); return; }
			if (!File.Exists(filePath)) {
				var file = File.Create(filePath);
				file.Close();
				Debug.Log(filePath + " now exists");
			}
			SaveGrid saveGrid = new SaveGrid();
			saveGrid.grid = nodeGrid;
			Debug.Log("Saving " + nodeGrid.Length + " nodes.");
			DataFormat format = DataFormat.Binary;
			var bytes = SerializationUtility.SerializeValue(saveGrid, format);
			File.WriteAllBytes(filePath, bytes);
#endif
		}

		public void LoadData() {
			attemptedLoad = true;

			if (!File.Exists(filePath)) { Debug.Log($"Failed to Load from {filePath}."); return; }

			DataFormat format = DataFormat.Binary;
			var bytes = File.ReadAllBytes(filePath);
			SaveGrid saveGrid = SerializationUtility.DeserializeValue<SaveGrid>(bytes, format);
			Debug.Log("Loaded");
			if (saveGrid == null || saveGrid.grid == null) { Debug.Log("Failed to Load."); return; }
			nodeGrid = saveGrid.grid;

		}

	}

	[System.Serializable]
	public class SaveGrid {
		[OdinSerialize] public Node[,] grid;
	}
}
