using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;

public class ClosestNodeDebug : MonoBehaviour
{

	void OnDrawGizmosSelected() {
		if (NodeGrid2D.instance == null) { return; }
		if (NodeGrid2D.instance.DontDraw) { return; }
		if (Camera.main == null) { return; }

		var mousePos = Input.mousePosition;
		var pos = Camera.main.ScreenToWorldPoint(mousePos);
		var node = NodeGrid2D.instance.NodeFromWorldPoint(pos);

		Gizmos.color = Color.green;
		Gizmos.DrawCube(NodeGrid2D.instance.PosFromNode(node), Vector3.one * 0.5f);
		if (node.walkable) {
			Gizmos.color = Color.magenta;
			Gizmos.DrawCube(NodeGrid2D.instance.PosFromNode(node), Vector3.one * 0.45f);
		}
		else {
			var closestNode = NodeHelper.ClosestWalkable(node);
			Gizmos.color = Color.magenta;
			Gizmos.DrawCube(NodeGrid2D.instance.PosFromNode(closestNode), Vector3.one * 0.45f);
		}
	}
}
