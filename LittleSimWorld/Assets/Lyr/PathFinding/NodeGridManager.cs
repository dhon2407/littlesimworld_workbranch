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

		List<Node> tempUnwalkableNodes_HIGH = new List<Node>(10000);
		List<Node> tempUnwalkableNodes_MEDIUM = new List<Node>(10000);
		List<Node> tempUnwalkableNodes_LOW = new List<Node>(10000);

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

        private void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == 0)//Main Menu index = 0
                Destroy(instance.gameObject);
        }
        #endregion

        public static NodeGrid2D GetGrid(Resolution resolution) => instance.Grids[resolution];

		public static void SetPosUnwalkable(Vector2 pos, Collider2D col) {
			AddToList(instance.tempUnwalkableNodes_HIGH, instance.Grids[Resolution.High]);
			AddToList(instance.tempUnwalkableNodes_MEDIUM, instance.Grids[Resolution.Medium]);
			//AddToList(instance.tempUnwalkableNodes_HIGH, instance.Grids[Resolution.High]);

			void AddToList(List<Node> list, NodeGrid2D grid) {
				var n = grid.NodeFromWorldPoint(pos);
				if (!n.isCurrentlyOccupied) { n.isCurrentlyOccupied = col; }
				if (list.Contains(n)) { return; }
				list.Add(n);
			}

		}

		void LateUpdate() => CheckUnwalkables();

		void CheckUnwalkables() {

			CheckList(tempUnwalkableNodes_HIGH, Grids[Resolution.High]);
			CheckList(tempUnwalkableNodes_MEDIUM, Grids[Resolution.Medium]);

			void CheckList(List<Node> list, NodeGrid2D grid) {
				for (int i = list.Count - 1; i >= 0; i--) {
					if (i < 0) { break; }
					var item = list[i];
					var pos = grid.PosFromNode(item);
					if (!grid.IsNodeWalkable_Temp(pos, mask)) { continue; }
					item.isCurrentlyOccupied = null;
					list.RemoveAt(i);
				}
			}
		}

		void OnDrawGizmosSelected() {

			DrawFromList(tempUnwalkableNodes_HIGH, Grids[Resolution.High]);
			DrawFromList(tempUnwalkableNodes_MEDIUM, Grids[Resolution.Medium]);

			void DrawFromList(List<Node> list, NodeGrid2D grid) {
				var size = grid.nodeSize * Vector2.one;
				Gizmos.color = Color.blue;
				foreach (var item in list) {
					Gizmos.DrawCube(grid.PosFromNode(item), size);
				}
			}
		}
	}
}