using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;

namespace Characters.RandomNPC {
	public class AppearCommand : INPCCommand {

		RandomNPC parent;

		static Camera cam;
		const int maxAttemptsForNewLocation = 20;

		public bool IsFinished { get; set; }
		public CommandInterval interval => CommandInterval.Update;

		public AppearCommand(RandomNPC parent) {
			this.parent = parent;
			cam = Camera.main;
		}

		public void Initialize() {
			IsFinished = true;
			SpawnOutOfView();
		}

		void SpawnOutOfView() {
			var camPos = (Vector2) GameLibOfMethods.player.transform.position;
			var sqrOrthoSize = cam.orthographicSize * cam.orthographicSize;

			var grid = NodeGridManager.GetGrid(PathFinding.Resolution.Medium);

			int currentAttempt = 0;
			while (currentAttempt <= maxAttemptsForNewLocation) {
				currentAttempt++;
				var rndNode = grid.GetRandomWalkable();

				// We don't want nodes that are occupied
				if (rndNode.isCurrentlyOccupied != null) { continue; }

				var loc = grid.PosFromNode(rndNode);
				var sqrMag = Vector2.SqrMagnitude(loc - camPos);

				// We don't want nodes that are within camera view;
				if (sqrMag <= 3 * sqrOrthoSize) { continue; }

				parent.transform.position = loc;
				return;
			}

			// We reach here if the max attempts limit has been exceeded
			// TODO: Make sure it never happens.. somehow..
			Debug.Log("Max Attempts limit on spawning NPC outside the camera's frustum has been exceeded.");

			// Safety for not appearing on the first frame
			// .. has to happen since the physics won't update until FixedUpdate, and we do this in Update
			parent.transform.position = Vector3.zero;
		}

		public void ExecuteCommand() { }

	}

	public class DisappearCommand : INPCCommand {

		RandomNPC parent;

		static Camera cam;

		public bool IsFinished { get; set; }
		public CommandInterval interval => CommandInterval.Update;

		public DisappearCommand(RandomNPC parent) {
			this.parent = parent;
			cam = Camera.main;
		}

		// TODO: Change or implement this targetAlpha variable 
		public void Initialize() {
			IsFinished = true;
			DisappearSafely();
		}


		void DisappearSafely() {
			var camPos = cam.transform.position;
			var pos = parent.transform.position;
			var sqrOrthoSize = cam.orthographicSize * cam.orthographicSize;

			var sqrMag = Vector2.SqrMagnitude(pos - camPos);
			if (sqrMag >= 3 * sqrOrthoSize) { return; }

			var randomNode = NodeGridManager.GetGrid(PathFinding.Resolution.Medium).GetRandomWalkable();
			var newFadePos = NodeGridManager.GetGrid(PathFinding.Resolution.Medium).PosFromNode(randomNode);

			parent.commandQueue.Enqueue(new HangAroundCommand(parent, Random.Range(1, 5f)));
			parent.commandQueue.Enqueue(new MoveToCommand(parent, newFadePos));
			parent.commandQueue.Enqueue(new DisappearCommand(parent));
		}

		// TODO: Make this applicable for fading away, like for going to work or whatnot.
		public void ExecuteCommand() {
			//t += Speed * Time.deltaTime;
			// parent.SetAlpha(
		}

	}



}