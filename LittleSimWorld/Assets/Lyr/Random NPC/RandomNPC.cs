using System.Collections.Generic;
using CharacterData;
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

		INPCCommand currentCommand;

		void Awake() {
			anim = GetComponentInChildren<Animator>();
			rb = GetComponent<Rigidbody2D>();
			col = GetComponent<Collider2D>();
			commandQueue = new Queue<INPCCommand>();
			//col.isTrigger = false;
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
				Physics2D.SyncTransforms();
				currentCommand.Initialize();
			}
			else {
				currentCommand = null;
				OnCompleteAction();
			}
		}

	}
}