using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;

namespace Characters.RandomNPC {
	public class MoveToCommand : INPCCommand {

		RandomNPC parent;
		Vector2 Target;

		List<Node> path;

		const PathFinding.Resolution resolution = PathFinding.Resolution.Medium;
		NodeGrid2D grid => NodeGridManager.GetGrid(resolution);
		Node[,] nodeGrid => grid.nodeGrid;

		Rigidbody2D rb => parent.rb;
		Animator anim => parent.anim;
		Collider2D col => parent.col;

		public bool IsFinished { get; set; }
		public CommandInterval interval => CommandInterval.FixedUpdate;

		public MoveToCommand(RandomNPC parent, Vector2 Target) {
			this.parent = parent;
			this.Target = Target;
		}

		public void Initialize() {
			path = new List<Node>(1000);
			GetPath();
			anim.Play("Walk");
		}

		void GetPath() {
			RequestPath.GetPath_Avoidance(rb.position, Target, path, resolution, col);
			index = 0;
		}

		float Speed = 1.5f;
		int index = 0;

		public void ExecuteCommand() {

			if (index >= path.Count) {
				IsFinished = true;
				anim.Play("Idle");
				return;
			}

			if (!CanWalkAt(path[index]) || (index < path.Count - 1 && !CanWalkAt(path[index + 1]))) {
				GetPath();
				return;
			}

			var spd = Speed * Time.fixedDeltaTime * GameTime.Clock.TimeMultiplier;
			anim.Play("Walk");

			NodeGridManager.SetPosUnwalkable(rb.position, col);

			var targetPos = grid.PosFromNode(path[index]);
			var offset = targetPos - rb.position;

			var posAfterMovement = Vector2.MoveTowards(rb.position, targetPos, spd);

			if (posAfterMovement == targetPos) {
				index++;
				if (index < path.Count) {
					if (!CanWalkAt(path[index])) { }
					else {
						targetPos = grid.PosFromNode(path[index]);
						offset = targetPos - rb.position;
						posAfterMovement = Vector2.MoveTowards(rb.position, targetPos, spd);
					}
				}
			}

			rb.MovePosition(posAfterMovement);

			if (index >= path.Count) {
				IsFinished = true;
				anim.Play("Idle");
			}
			else {
				CalculateFacing(offset);
			}

		}


		void CalculateFacing(Vector2 offset) {
			bool isYBigger = Mathf.Abs(offset.x) <= 0.01f;//Mathf.Abs(offset.x) < Mathf.Abs(offset.y);

			if (offset == Vector2.zero) { return; }

			if (isYBigger) {
				if (offset.y > 0) { parent.visualsHelper.FaceUP(); }
				else if (offset.y < 0) { parent.visualsHelper.FaceDOWN(); }

				if (offset.y != 0) { anim.SetFloat("Vertical", Mathf.Sign(offset.y)); }
				else { anim.SetFloat("Vertical", 0); }

				anim.SetFloat("Horizontal", 0);
			}
			else {
				if (offset.x > 0) { parent.visualsHelper.FaceRIGHT(); }
				else if (offset.x < 0) { parent.visualsHelper.FaceLEFT(); }

				if (offset.x != 0) { anim.SetFloat("Horizontal", Mathf.Sign(offset.x)); }
				else { anim.SetFloat("Horizontal", 0); }

				anim.SetFloat("Vertical", 0);
			}
		}

		bool CanWalkAt(Node node) => node.isCurrentlyOccupied == null || node.isCurrentlyOccupied == col;

	}
}