namespace PathFinding {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Sirenix.OdinInspector;
	using Sirenix.Serialization;
	using System;
	using System.Linq;

	public class NodeGrid2D : MonoBehaviour {

		#region Singleton			
		public static NodeGrid2D instance;
		void Awake() {
			if (!instance) { instance = this; }
			else if (instance != this) { Destroy(gameObject); }
			gridData.Load();
			CacheNeighbours();
			//BetterList list = new BetterList(ref nodeGrid[0,0]);
		}

		#endregion
		public enum _Tabbing { GridSettings, Walkable, DataSaving }
		//[EnumToggleButtons,HideLabel,GUIColor(0.9f,0,0.9f,0.9f)] public _Tabbing _tabbing;


		public Vector2 GridSize;
		[Range(0.1f, 3)] public float nodeSize = 1;
		public Vector2 center;

		public Color gridColor = new Color(0, 0, 1, 0.6f);
		public LayerMask unwalkableMask;
		public string DataName;
		public GridData gridData;

		#region EDITOR STUFF
#if (UNITY_EDITOR)
		//[ShowIf("_tabbing",_Tabbing.DataSaving)]
		[Button]
		public void Save() {
			if (gridData == null || String.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(gridData))) {
				gridData = GridData.CreateInstance<GridData>();
				string path = "Assets/GridData/" + DataName + ".asset";
				UnityEditor.AssetDatabase.CreateAsset(gridData, path);
				gridData.name = DataName;
				FullColliderCheck();

				UnityEditor.EditorUtility.SetDirty(gridData);
				UnityEditor.AssetDatabase.SaveAssets();
			}
			else { gridData.Save(); }
		}

		[Button]
		public void Load() {
			if (gridData != null) { gridData.Load(); }
		}

		[Button]
		public void FullColliderCheck() {
			gridData.nodeGrid = new Node[gridX, gridY];
			gridData.nodeGrid[0, 0] = new Node(false, Center - GridSize / 2, 0, 0);

			for (int x = 0; x < gridX; x++) {
				for (int y = 0; y < gridY; y++) {
					Vector2 pos = (Center - GridSize / 2) + new Vector2(x * nodeSize, y * nodeSize);
					bool walkable = !Physics2D.OverlapCircle(pos, (nodeSize / 2) - 0.1f, unwalkableMask);
					gridData.nodeGrid[x, y] = new Node(walkable, pos, x, y);
				}
			}
		}

		public void ResizeGrid() {

			if (gridX <= 0 || gridY <= 0) { Debug.Log("BAD INPUT"); return; }
			List<Node> unwalkableNodes = new List<Node>(nodeGrid.Length);
			foreach (var n in nodeGrid) { if (!n.walkable) { unwalkableNodes.Add(n); } }
			gridData.nodeGrid = new Node[gridX, gridY];
			gridData.nodeGrid[0, 0] = new Node(false, Center - GridSize / 2, 0, 0);

			for (int x = 0; x < gridX; x++) {
				for (int y = 0; y < gridY; y++) {
					Vector2 pos = (Center - GridSize / 2) + new Vector2(x * nodeSize, y * nodeSize);
					bool walkable = true; //DONT ADD NEW DATA
					gridData.nodeGrid[x, y] = new Node(walkable, pos, x, y);
				}
			}

			foreach (var n in unwalkableNodes) { NodeFromWorldPoint(n.worldPosition).walkable = false; }
		}

		[HideInInspector, NonSerialized] public List<Node> currentlySelected = new List<Node>();
		[HideInInspector, NonSerialized] public Node selectedNode;
		public void OnDrawGizmos() {
			if (DontDraw || nodeGrid == null) { return; }
			Gizmos.DrawWireCube(Center, GridSize);
			Color walkableColor = new Color(1, 1, 1, 0.1f);
			Color unwalkableColor = new Color(1, 0, 0, 0.4f);

			var size = Vector2.one * nodeSize;

			foreach (var item in nodeGrid) {
				if (selectedNode == item) {
					//Gizmos.color = Color.yellow;
					Gizmos.color = new Color(1, 0.9f, 0, 0.3f);
					Gizmos.DrawCube(PosFromNode(item), size);
				}
				if (!item.walkable) {
					Gizmos.color = unwalkableColor;
					Gizmos.DrawWireCube(PosFromNode(item), size);
				}
				else {
					Gizmos.color = walkableColor;
					Gizmos.DrawWireCube(PosFromNode(item), size);
				}
			}

			foreach (var n in currentlySelected) {
				Gizmos.color = new Color(0, 1, 0, 0.3f);
				Gizmos.DrawCube(PosFromNode(n), size);
			}

		}

#endif
		#endregion

		void CacheNeighbours() { foreach (var item in nodeGrid) { item.Cache(this); } }

		public Node[,] nodeGrid => gridData?.nodeGrid;

		public Vector2 Center {
			get { return (Vector2) transform.position + center; }
			set { center = value - (Vector2) transform.position; }
		}


		public int gridX => Mathf.FloorToInt(GridSize.x / nodeSize) + 1;
		public int gridY => Mathf.FloorToInt(GridSize.y / nodeSize) + 1;

		public Node NodeFromWorldPoint(Vector2 worldPosition) {

			float percentX = (worldPosition.x - Center.x + GridSize.x / 2) / GridSize.x;
			float percentY = (worldPosition.y - Center.y + GridSize.y / 2) / GridSize.y;

			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);

			int x = Mathf.RoundToInt((gridX - 1) * percentX);
			int y = Mathf.RoundToInt((gridY - 1) * percentY);

			if (nodeGrid.GetLength(0) <= x) { Debug.Log("WRONG X"); return null; }
			if (nodeGrid.GetLength(1) <= y) { Debug.Log("WRONG Y"); return null; }

			return nodeGrid[x, y];
		}

		public Vector2 PosFromCoords(int x, int y) => (Center - GridSize / 2) + (new Vector2(x, y) * nodeSize);
		public Vector2 PosFromNode(Node node) => PosFromCoords(node.X, node.Y);

		[Serializable]
		public class TerrainType {
			public LayerMask terrainMask;
			public int terrainPenalty;
		}

		public bool ClosestHelp = false;
		public bool DontDraw = false;
	}

	public static class NodeHelper {
		static List<Node> helperList = new List<Node>(80);
		static Node[,] nodeGrid;

		const int maxTries = 10;
		const int maxRadius = 5;
		static int X, Y, maxX, maxY;


		public static Node ClosestWalkable(Node closestTo) {


			nodeGrid = NodeGrid2D.instance.nodeGrid;

			// First, check if any of the neighbors are walkable
			BaseNode[] neighbors = closestTo.neighbours.directionNeighbours;

			X = closestTo.X;
			Y = closestTo.Y;

			maxX = nodeGrid.GetLength(0) - 1;
			maxY = nodeGrid.GetLength(1) - 1;

			helperList.Clear();


			// Find Closest Left
			helperList.Add(ClosestToOrigin(Decrease, Stay));

			// Find Closest Right
			helperList.Add(ClosestToOrigin(Increase, Stay));

			// Find Closest Top
			helperList.Add(ClosestToOrigin(Stay, Increase));

			// Find Closest Bot
			helperList.Add(ClosestToOrigin(Stay, Decrease));

			// Find Closest Diags
			helperList.Add(ClosestToOrigin(Decrease, Decrease));
			helperList.Add(ClosestToOrigin(Decrease, Increase));
			helperList.Add(ClosestToOrigin(Increase, Decrease));
			helperList.Add(ClosestToOrigin(Increase, Increase));

			// Find Most efficient Node
			int minDist = int.MaxValue;
			Node closestNode = null;

			foreach (Node n in helperList) {
				if (n == null) { continue; }

				int dist = heuristic(n, closestTo);
				if (dist >= minDist) { continue; }

				minDist = dist;
				closestNode = n;
			}

			if (closestNode == null) {
				Debug.LogError("Couldn't find valid node");
				return closestTo;
			}
			else { return closestNode; }

		}

		#region Helper Methods
		static Node ClosestToOrigin(Func<int, int> opX, Func<int, int> opY) {
			int x = X;
			int y = Y;

			for (int i = 0; i < maxTries; i++) {
				x = opX(x);
				y = opY(y);

				if (y < 0 || y > maxY) { break; }
				if (x < 0 || x > maxX) { break; }

				Node n = nodeGrid[x, y];
				if (n.walkable) { return n; }
			}

			return null;
		}

		static int Increase(int a) => a + 1;
		static int Decrease(int a) => a - 1;
		static int Stay(int a) => a;

		static int heuristic(Node A, Node B) {
			int distX = (A.X > B.X ? A.X - B.X : B.X - A.X);
			int distY = (A.Y > B.Y ? A.Y - B.Y : B.Y - A.Y);

			return distX + distY;
		}
		#endregion

		//public static TSource First<TSource>(this IList<TSource> source, Func<TSource, bool> predicate) {
		//	for (int i = 0; i < source.Count; i++) {
		//		if (predicate(source[i])) { return source[i]; }
		//	}
		//	return default(TSource);
		//}

	}

}
