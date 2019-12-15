using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PathFinding {
	public static class RequestPath {

		#region INITIALIZATION
		static NodeGrid2D grid;			static Node[,] nodeGrid;
		static OpenHeap openSet;		static ClosedHeap closedSet;

		static RequestPath() {
			grid = NodeGrid2D.instance;
			nodeGrid = grid.nodeGrid;
			openSet = new OpenHeap(grid.gridX * grid.gridY);
			closedSet = new ClosedHeap(grid.gridX * grid.gridY);
		}

		#endregion

		public static void GetPath(Vector2 start,Vector2 target,List<Node> pp) {

			#region INIT
			if (grid == null) {
				grid = NodeGrid2D.instance; 
				nodeGrid = grid.nodeGrid;
			}

			Node startNode = grid.NodeFromWorldPoint(start);
			Node targetNode = grid.NodeFromWorldPoint(target);

			if (!startNode.walkable) { startNode = NodeHelper.ClosestWalkable(startNode); }
			if (!targetNode.walkable) { targetNode = NodeHelper.ClosestWalkable(targetNode); }

			Node node, nbr;
			openSet.Clear();
			closedSet.Clear();
			openSet.Add(startNode);
			startNode.hCost = startNode.gCost = 0;
			#endregion

			while (openSet.Count > 0) {
				node = openSet.RemoveFirst();
				closedSet.Add(node);

				if (node == targetNode) {
					pp.Clear();
					Node currentNode = targetNode;
					while (currentNode != startNode) {
						pp.Add(currentNode);
						currentNode = nodeGrid[currentNode.parent.x, currentNode.parent.y];
					}
					pp.Reverse();
					return;
				}

				bool skipD = false;

				var nbsA = node.neighbours.directionNeighbours;
				int scanA = node.neighbours.DirectionCount;
				for (int i = 0; i < scanA; i++) {
					nbr = nodeGrid[nbsA[i].x,nbsA[i].y];
					if (!nbr.walkable) { skipD = true; continue; }
					if (closedSet.Contains(nbr)) { continue; }

					int cost = node.gCost + heuristic(node,nbr);
					if (!openSet.Contains(nbr)) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr,targetNode);
						nbr.parent = new BaseNode(node.X,node.Y);
						openSet.Add(nbr);
					}
					else if (cost < nbr.gCost) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr,targetNode);
						nbr.parent = new BaseNode(node.X,node.Y);
					}
				}
				if (skipD) { continue; }

				var nbsB = node.neighbours.diagonalNeighbours;
				int scanB = node.neighbours.DiagonalCount;

				for (int i = 0; i < scanB; i++) {
					nbr = nodeGrid[nbsB[i].x, nbsB[i].y];
					if (!nbr.walkable) { continue; }
					if (closedSet.Contains(nbr)) { continue; }

					for (int j = 0; j < nbr.neighbours.DiagonalCount; j++) {
						var nbr_D = nbr.neighbours.diagonalNeighbours[j];
						var dNode = nodeGrid[nbr_D.x, nbr_D.y];
						if (!dNode.walkable) {
							break;
						}
					}

					int cost = node.gCost + heuristic(node, nbr);
					if (!openSet.Contains(nbr)) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
						openSet.Add(nbr);
					}
					else if (cost < nbr.gCost) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
					}
				}
			}

		} 

		static int heuristic(Node A,Node B) {
			// Performant Mathf.Abs 
			int distX = (A.X > B.X ? A.X - B.X : B.X - A.X);
			int distY = (A.Y > B.Y ? A.Y - B.Y : B.Y - A.Y);

			// 14 == 10 * ~sqrt(2)
			if (distX > distY) { return 14 * distY + 10 * (distX - distY); }
			else { return 14 * distX + 10 * (distY - distX); }
		}

	}
}
