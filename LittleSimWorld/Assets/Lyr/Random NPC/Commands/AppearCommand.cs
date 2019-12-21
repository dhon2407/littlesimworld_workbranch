using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;

namespace Characters.RandomNPC {
	public class AppearCommand : INPCCommand {

		RandomNPC parent;
		float targetAlpha;
		float startAlpha;

		static List<RandomNPCLocationHelper.NPCLocation> spawnList;
		static Camera cam;
		const int maxAttemptsForNewLocation = 5;

		Rigidbody2D rb => parent.rb;
		Animator anim => parent.anim;

		public bool IsFinished { get; set; }
		public CommandInterval interval => CommandInterval.Update;

		public AppearCommand(RandomNPC parent,List<RandomNPCLocationHelper.NPCLocation> spawnList, float targetAlpha) {
			this.parent = parent;
			this.targetAlpha = targetAlpha;
			AppearCommand.spawnList = spawnList;
			cam = Camera.main;
		}

		// TODO: Change or implement this targetAlpha variable 
		public void Initialize() {
			IsFinished = true;
			if (targetAlpha == 0) { DisappearSafely(); }
			else { SpawnOutOfView(); }
		}

		void SpawnOutOfView() {
			var camPos = (Vector2) cam.transform.position;
			var sqrOrthoSize = cam.orthographicSize * cam.orthographicSize;

			int currentAttempt = 0;
			while (currentAttempt <= maxAttemptsForNewLocation) {
				var rnd = Random.Range(0, spawnList.Count);
				var loc = spawnList[rnd];
				var sqrMag = Vector2.SqrMagnitude(loc - camPos);

				if (sqrMag <= sqrOrthoSize) {
					currentAttempt++;
					continue;
				}

				parent.transform.position = loc;
				return;
			}

			// We reach here if the max attempts limit has been exceeded
			Debug.Log("Max Attempts limit on spawning NPC outside the camera's frustum has been exceeded.");
			parent.transform.position = spawnList[Random.Range(0, spawnList.Count)];
		}

		void DisappearSafely() {
			var camPos = cam.transform.position;
			var pos = parent.transform.position;
			var sqrOrthoSize = cam.orthographicSize * cam.orthographicSize;

			var sqrMag = Vector2.SqrMagnitude(pos - camPos);
			if (sqrMag >= sqrOrthoSize) { return; }

			var newFadePos = spawnList[Random.Range(0, spawnList.Count)];

			parent.commandQueue.Enqueue(new HangAroundCommand(parent, Random.Range(1, 5f)));
			parent.commandQueue.Enqueue(new MoveToCommand(parent, newFadePos));
			parent.commandQueue.Enqueue(new AppearCommand(parent, spawnList, 0));
		}



		// TODO: Make this applicable for fading away, like for going to work or whatnot.
		public void ExecuteCommand() {
			//t += Speed * Time.deltaTime;
			// parent.SetAlpha(
		}

	}
}