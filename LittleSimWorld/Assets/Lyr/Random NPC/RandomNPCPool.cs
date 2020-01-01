using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Characters.RandomNPC {
	public class RandomNPCPool : SerializedMonoBehaviour {
		public uint PoolAmount;
		[MaxValue("PoolAmount")] public uint MaxActive;

		[Space] public RandomNPC npcPrefab;

		public static RandomNPCPool instance;

		public RandomNPCLocationHelper npcLocationHelper;

		public List<Collider2D> NormallyIgnoredColliders;

		Queue<RandomNPC> npcPool;

		#region Initialization and Editor

		void Awake() {
			instance = this;
			InitializePool();
		}

		void InitializePool() {
			npcPool = new Queue<RandomNPC>((int)PoolAmount);

			for (int i = 0; i < PoolAmount; i++) {
				var newObj = GameObject.Instantiate(npcPrefab);
				newObj.gameObject.name = "Random NPC";
				newObj.transform.SetParent(transform);
				newObj.gameObject.SetActive(false);

				newObj.OnCompleteAction = () => { npcPool.Enqueue(newObj); newObj.gameObject.SetActive(false); };
				newObj.visualsHelper.Initialize();

				npcPool.Enqueue(newObj);
			}
		}
		//void OnDrawGizmosSelected() {
		//	Gizmos.color = Color.white;
		//	foreach (var loc in npcLocationHelper.SpawnLocations) {
		//		Gizmos.DrawWireSphere(loc, 0.2f);
		//	}
		//	Gizmos.color = Color.magenta;
		//	foreach (var loc in npcLocationHelper.GoToLocations) {
		//		Gizmos.DrawWireSphere(loc, 0.4f);
		//	}
		//}
		#endregion

		float t;
		public float Interval = 2f;
		void Update() {
			t += Time.deltaTime;
			if (t >= Interval) {
				FireNext();
				t -= Interval;
			}
		}

		void FireNext() {
			if (npcPool.Count <= 0) { return; }
			if (npcPool.Count <= PoolAmount - MaxActive) { return; }

			T RandomFromList<T>(List<T> list) => list[Random.Range(0, list.Count)];

			Vector2 target = RandomFromList(npcLocationHelper.GoToLocations);
			Vector2 origin = RandomFromList(npcLocationHelper.SpawnLocations);
			target += new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 5f));

			float time = Random.Range(1f, 20f);

			var nextNPC = npcPool.Dequeue();

			// If the command queue is not empty there is an error in code.
			if (nextNPC.commandQueue.Count != 0) { Debug.LogWarning("Command queue of {nextNPC} is not empty."); }

			nextNPC.commandQueue.Enqueue(new AppearCommand(nextNPC));
			nextNPC.commandQueue.Enqueue(new MoveToCommand(nextNPC, target));
			nextNPC.commandQueue.Enqueue(new HangAroundCommand(nextNPC, time));
			nextNPC.commandQueue.Enqueue(new MoveToCommand(nextNPC, origin));
			nextNPC.commandQueue.Enqueue(new DisappearCommand(nextNPC));

			// Patch for the single frame that the NPC appears on the screen before searching for a new location.
			nextNPC.transform.position = Vector3.one * 1000;
			nextNPC.gameObject.SetActive(true);
			nextNPC.GetNextCommand();
		}

	}
}