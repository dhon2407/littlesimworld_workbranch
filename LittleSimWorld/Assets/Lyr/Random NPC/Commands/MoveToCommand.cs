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
		float Speed = 1.3f;

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
			GetPath(true);
			anim.Play("Walk");
		}


		void GetPath(bool forcePath = false) {
			if (!forcePath && !pathTimer.IsReady(false)) {
				gotValidPath = false;
				anim.Play("Idle");
				return;
			}

			currentPathNodeCount = path.Count - index;
			shortPathNodeCount = Mathf.Min(shortPathNodeCount, currentPathNodeCount);

			RequestPath.GetPath_Avoidance(rb.position, Target, path, resolution, col);

			currentPathNodeCount = path.Count;
			shortPathNodeCount = Mathf.Min(shortPathNodeCount, currentPathNodeCount);

			index = 0;
			gotValidPath = path.Count > 0;
			pathTimer.ForceUpdate();
		}

		public void ExecuteCommand() {

			FrameSafeEnsurance();

			if (gotValidPath && index >= path.Count) {
				IsFinished = true;
				anim.Play("Idle");
				return;
			}

			// Local avoidance implementation
			if (ShouldGetNewPath()) {
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

		#region Path Optimizations

		int previousFrameCount;
		bool hasUpdatedThisFrame = false;
		const int frameDelay = 3;
		void FrameSafeEnsurance() {
			// Optimization -- only update visuals once per few frames
			int currentFrameCount = Time.frameCount;
			if (previousFrameCount >= currentFrameCount - frameDelay) { return; }
			previousFrameCount = currentFrameCount;

			hasUpdatedThisFrame = false;
		}

		int index = 0;
		bool gotValidPath;
		int shortPathNodeCount = 10000;
		int currentPathNodeCount = 10000;
		const int maxAllowedPathExtents = 10;
		FrameTimer pathTimer = new FrameTimer(3);

		bool gotEfficientPath => shortPathNodeCount <= currentPathNodeCount + maxAllowedPathExtents;

		bool ShouldGetNewPath() {
			if (!gotValidPath) { return true; }
			bool isOnUnwalkablePath = !CanWalkAt(path[index]) || (index < path.Count - 1 && !CanWalkAt(path[index + 1]));
			if (isOnUnwalkablePath) { return true; }
			if (!gotEfficientPath && !pathTimer.IsReady(true)) { return true; }

			return false;
		}




		#endregion


		class FrameTimer {
			static int currentFrameCount => Time.frameCount;
			int previousFrameCount;
			int frameDelay;

			static FrameTimer() {
				/// TODO: Make currentFrameCount update once per frame
			}

			public FrameTimer(int frameDelay) {
				previousFrameCount = currentFrameCount;
				this.frameDelay = frameDelay;
			}

			public bool IsReady(bool update = false) {
				if (previousFrameCount >= currentFrameCount - frameDelay) { return false; }
				if (update) { ForceUpdate(); }
				return true;
			}

			public void ForceUpdate() => previousFrameCount = currentFrameCount;
		}

	}
}