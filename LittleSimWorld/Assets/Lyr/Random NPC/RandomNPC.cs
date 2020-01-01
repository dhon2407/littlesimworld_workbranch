using System.Collections.Generic;
using CharacterData;
using PathFinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.RandomNPC {
	[RequireComponent(typeof(CapsuleCollider2D), typeof(Rigidbody2D))]
	public class RandomNPC : SerializedMonoBehaviour {


		public VisualsHelper visualsHelper;

		[System.NonSerialized] public System.Action OnCompleteAction;
		[System.NonSerialized] public Queue<INPCCommand> commandQueue;
		[System.NonSerialized] public Rigidbody2D rb;
		[System.NonSerialized] public Animator anim;
		[System.NonSerialized] public Collider2D col;
		[System.NonSerialized] public List<Node> path;

		INPCCommand currentCommand;
		static List<Collider2D> ignoreColliders => RandomNPCPool.instance.NormallyIgnoredColliders;
		static Collider2D wallCollider;

		void Awake() {
			anim = GetComponentInChildren<Animator>();
			rb = GetComponent<Rigidbody2D>();
			col = GetComponent<Collider2D>();
			commandQueue = new Queue<INPCCommand>();
			path = new List<Node>(10000);
			col.isTrigger = false;
			if (!wallCollider) { wallCollider = GameObject.Find("Walls").GetComponent<Collider2D>(); }
		}

		void Update() {
			if (currentCommand == null) { return; }
			if (currentCommand.interval != CommandInterval.Update) { return; }
			if (!currentCommand.IsFinished) { currentCommand.ExecuteCommand(); }
			else { GetNextCommand(); }
		}

		void FixedUpdate() {
			if (currentCommand == null) { return; }
			if (currentCommand.interval != CommandInterval.FixedUpdate) { return; }
			if (!currentCommand.IsFinished) { currentCommand.ExecuteCommand(); }
			else { GetNextCommand(); }
		}


		public void GetNextCommand() {
			if (commandQueue.Count > 0) {
				currentCommand = commandQueue.Dequeue();
				//Physics2D.SyncTransforms();
				currentCommand.Initialize();
			}
			else {
				currentCommand = null;
				OnCompleteAction();
			}
		}

		void OnCollisionStay2D(Collision2D collision) {
			var hit = collision.gameObject;
			if (hit == GameLibOfMethods.player) { Physics2D.IgnoreCollision(col, wallCollider, false); }
			//else if (hit.layer == LayerMask.GetMask("Cars")) { Physics2D.IgnoreCollision(col, wallCollider, true); }
		}

		void OnCollisionExit2D(Collision2D collision) {
			var hit = collision.gameObject;
			if (hit == GameLibOfMethods.player) { Physics2D.IgnoreCollision(col, wallCollider, true); }
			//else if (hit.layer == LayerMask.GetMask("Cars")) { Physics2D.IgnoreCollision(col, wallCollider, false); }
		}
	}
}