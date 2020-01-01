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

		Rigidbody2D rb;
		Animator anim;
		Collider2D col;

		public bool IsFinished { get; set; }
		public CommandInterval interval => CommandInterval.FixedUpdate;

		public MoveToCommand(RandomNPC parent, Vector2 Target) {
			this.parent = parent;
			this.Target = Target;
			this.rb = parent.rb;
			this.anim = parent.anim;
			this.col = parent.col;
			this.path = parent.path;
		}

		public void Initialize() {
			GetPath();
			anim.Play("Walk");
		}

		void GetPath() {
			if (gotPathThisFrame) {
				gotValidPath = false;
				return;
			}
			RequestPath.GetPath_Avoidance(rb.position, Target, path, resolution, col);
			index = 0;
			gotValidPath = true;
			gotPathThisFrame = true;
		}

		float Speed = 1.5f;
		int index = 0;
		bool gotValidPath;

		public void ExecuteCommand() {

			FrameSafeEnsurance();

			if (index >= path.Count) {
				IsFinished = true;
				anim.Play("Idle");
				return;
			}


			// Local avoidance implementation
			if (!CanWalkAt(path[index]) || (index < path.Count - 1 && !CanWalkAt(path[index + 1]))) {
				GetPath();
				return;
			}

			if (!gotValidPath) { return; }

			var spd = Speed * Time.fixedDeltaTime * GameTime.Clock.TimeMultiplier;
			anim.Play("Walk");

			var pos = rb.position;
			var targetPos = grid.PosFromNode(path[index]);
			var offset = targetPos - pos;

			var posAfterMovement = Vector2.MoveTowards(pos, targetPos, spd);

			if (posAfterMovement == targetPos) {
				index++;
				if (index < path.Count) {
					if (!CanWalkAt(path[index])) { }
					else {
						targetPos = grid.PosFromNode(path[index]);
						offset = targetPos - pos;
						posAfterMovement = Vector2.MoveTowards(pos, targetPos, spd);
					}
				}
			}

			rb.MovePosition(posAfterMovement);
			if (!hasUpdatedThisFrame) { NodeGridManager.SetPosUnwalkable(pos, col); }

			if (index >= path.Count) {
				IsFinished = true;
				anim.Play("Idle");
			}
			else {
				CalculateFacing(offset);
			}

			hasUpdatedThisFrame = true;
		}

		void CalculateFacing(Vector2 offset) {

			if (hasUpdatedThisFrame) { return; }

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

		int previousFrameCount;
		bool hasUpdatedThisFrame = false;
		bool gotPathThisFrame = false;
		void FrameSafeEnsurance() {
			// Optimization -- only update visuals once per couple frames
			int currentFrameCount = Time.frameCount;
			if (previousFrameCount >= currentFrameCount - 2) { return; }
			previousFrameCount = currentFrameCount;

			hasUpdatedThisFrame = false;
			gotPathThisFrame = false;
		}
	}
}