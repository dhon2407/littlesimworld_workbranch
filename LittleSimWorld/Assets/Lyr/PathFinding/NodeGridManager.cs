namespace PathFinding {
	using System.Collections;
	using System.Collections.Generic;
	using PathFinding;
	using Sirenix.OdinInspector;
	using UnityEngine;
    using UnityEngine.SceneManagement;

    public enum Resolution { Low, Medium, High }

	[DefaultExecutionOrder(999999)]
	public class NodeGridManager : SerializedMonoBehaviour {


		public Dictionary<Resolution, NodeGrid2D> Grids = new Dictionary<Resolution, NodeGrid2D>().InitializeDefaultValues();

		[Space] public LayerMask mask;
		public int MinCheckAmount = 10;

		[ShowInInspector] public int Count => tempUnwalkableNodes_HIGH.Count + tempUnwalkableNodes_MEDIUM.Count + tempUnwalkableNodes_LOW.Count;

		HashSet<Node> tempUnwalkableNodes_HIGH = new HashSet<Node>();
		HashSet<Node> tempUnwalkableNodes_MEDIUM = new HashSet<Node>();
		HashSet<Node> tempUnwalkableNodes_LOW = new HashSet<Node>();
		HashSet<Node> tempNodeSet = new HashSet<Node>();

		static Collider2D playerCol;
		#region Singleton
		public static NodeGridManager instance;

		void Awake() {
			if (instance) {
				DestroyImmediate(gameObject);
				return;
			}
			else {
				instance = this;
				DontDestroyOnLoad(gameObject);
			}

			SceneManager.activeSceneChanged += DestroyOnMenuScreen;
			playerCol = GameLibOfMethods.player.GetComponent<Collider2D>();
		}

		private void DestroyOnMenuScreen(Scene oldScene, Scene newScene) {
			if (newScene.buildIndex == 0)//Main Menu index = 0
				Destroy(instance.gameObject);
		}
		#endregion


		#region Static functionality
		public static NodeGrid2D GetGrid(Resolution resolution) => instance.Grids[resolution];
		public static void SetPosUnwalkable(Vector2 pos, Collider2D col) {
			AddToList(instance.tempUnwalkableNodes_HIGH, instance.Grids[Resolution.High]);
			AddToList(instance.tempUnwalkableNodes_MEDIUM, instance.Grids[Resolution.Medium]);
			//AddToList(instance.tempUnwalkableNodes_HIGH, instance.Grids[Resolution.High]);

			void AddToList(HashSet<Node> list, NodeGrid2D grid) {
				var n = grid.NodeFromWorldPoint(pos);
				if (!n.isCurrentlyOccupied) { n.isCurrentlyOccupied = col; }
				if (list.Contains(n)) { return; }
				list.Add(n);
			}

		}
		public static void RegisterUnwalkable(Collider2D col) {
			Vector2 center = col.bounds.center;
			Vector2 extents = col.bounds.extents;

			var nodeSize = instance.Grids[Resolution.High].nodeSize;

			int iterationsX = Mathf.RoundToInt(extents.x / nodeSize);
			int iterationsY = Mathf.RoundToInt(extents.y / nodeSize);

			for (int x = -iterationsX; x <= iterationsX; x++) {
				for (int y = -iterationsY; y <= iterationsY; y++) {
					var dif = new Vector2(x, y) * nodeSize;
					SetPosUnwalkable(center + dif, col);
				}
			}
		}
		#endregion

		void LateUpdate() => CheckUnwalkables();
		void CheckUnwalkables() {
			CheckForChanges(tempUnwalkableNodes_HIGH, Grids[Resolution.High]);
			CheckForChanges(tempUnwalkableNodes_MEDIUM, Grids[Resolution.Medium]);

			void CheckForChanges(HashSet<Node> hashSet, NodeGrid2D grid) {
				tempNodeSet.Clear();

				// Turns out to be the most performant way.
				// compared with the alternative : hashSet.RemoveWhere(...)
				foreach (var node in hashSet) {
					var pos = grid.PosFromNode(node);
					if (!grid.IsNodeWalkable_Temp(pos, mask)) { continue; }
					tempNodeSet.Add(node);
				}
				foreach (var node in tempNodeSet) {
					node.isCurrentlyOccupied = null;
					hashSet.Remove(node);
				}
			}

		}

		/* Gizmos -- enable for Debug
		void OnDrawGizmosSelected() {

			DrawFromList(tempUnwalkableNodes_HIGH, Grids[Resolution.High]);
			DrawFromList(tempUnwalkableNodes_MEDIUM, Grids[Resolution.Medium]);

			void DrawFromList(HashSet<Node> list, NodeGrid2D grid) {
				var size = grid.nodeSize * Vector2.one;
				Gizmos.color = Color.blue;
				foreach (var item in list) {
					Gizmos.DrawCube(grid.PosFromNode(item), size);
				}
			}
		}
		*/
	}
}