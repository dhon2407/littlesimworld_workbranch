using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PlayerStats;

namespace PathFinding {
	[DefaultExecutionOrder(99999)]
	public class PlayerPathfinding : MonoBehaviour {

		Animator anim;
		Rigidbody2D rb;
		SpriteControler spr;
		GameObject player;
		NodeGrid2D grid;
		Collider2D col;

		public float Speed = 2;
		float builtUpSpeed = 0;

		void Start() {
			player = GameLibOfMethods.player;
			spr = FindObjectOfType<SpriteControler>();
			anim = GameLibOfMethods.animator;
			rb = player.GetComponent<Rigidbody2D>();
			grid = NodeGridManager.GetGrid(PathFinding.Resolution.High);
			col = player.GetComponent<Collider2D>();

			contactFilter = new ContactFilter2D();
			contactFilter.SetLayerMask(GameLibOfMethods.Instance.InteractablesLayer);
		}

		// TODO: Make current node occupied and use it for other agents 
		// To be added for NPCs to avoid pathing inside the player.
		Node currentNode;
		void MakeCurrentNodeOccupied() {
			NodeGridManager.SetPosUnwalkable(rb.position, col);
		}

		void LateUpdate() {
			CheckClicks();
			HandleBuiltUpSpeed();
			MakeCurrentNodeOccupied();
		}

		void CheckClicks() {
			bool shouldCancel = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && currentHandle.HasValue;
			if (shouldCancel) { Cancel(); }

			if (Input.GetMouseButtonDown(0)) {

				if (CustomInputModule.IsPointerBlocked(30)) { return; }

				var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				var interactable = CheckInteractable(pos);

				if (interactable != null) {
					if (currentHandle.HasValue) { Cancel(); }
					MoveTo(interactable.PlayerStandPosition, Speed, interactable.Interact);
				}
			}

			if (Input.GetMouseButtonDown(1)) {
				if (CustomInputModule.IsPointerBlocked(30)) { return; }

				if (currentHandle.HasValue) { Cancel(); }
				var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				MoveTo(pos, Speed, null);
			}
		}

		void Cancel() {
			if (currentHandle == null) { return; }
			MEC.Timing.KillCoroutines(currentHandle.Value);
			currentHandle = null;
			anim.SetBool("Walking", false);
			//IgnoreAllCollisionExcept(false);
			GameLibOfMethods.player.GetComponent<Collider2D>().isTrigger = false;
			PlayerAnimationHelper.ResetPlayer();
		}

		IInteractable CheckInteractable(Vector3 pos) {
			var colliders = new List<Collider2D>();

			Physics2D.OverlapCircle(pos, 0.1f, contactFilter, colliders);

			foreach (Collider2D collider in colliders) {
				var interactable = collider.GetComponent<IInteractable>();
				if (interactable == null) { continue; }

				return interactable;
			}
			return null;
		}

		void HandleBuiltUpSpeed() {
			if (currentHandle != null) {
				builtUpSpeed = Mathf.MoveTowards(builtUpSpeed, 1, Time.deltaTime);
			}
			else if (builtUpSpeed > 0) {
				builtUpSpeed = Mathf.MoveTowards(builtUpSpeed, 0, Time.deltaTime);
			}
		}

		#region Movement
		List<Node> path = new List<Node>(1000);
		ContactFilter2D contactFilter = new ContactFilter2D();
		MEC.CoroutineHandle? currentHandle;
		public void MoveTo(Vector3 pos,float speed, System.Action callback) {
			if (currentHandle.HasValue) { Cancel(); }
			if (GameLibOfMethods.doingSomething) { return; }
			currentHandle = WalkTo(pos, builtUpSpeed, speed, callback).Start(MEC.Segment.FixedUpdate);
		}


		IEnumerator<float> WalkTo(Vector3 Position, float StartingSpeedPercentage, float MaxSpeed, System.Action Callback) {

			PlayerAnimationHelper.StopPlayer();
			//RequestPath.GetPath(rb.position, Position, path, Resolution.High);
			RequestPath.GetPath_Avoidance(rb.position, Position, path, Resolution.High, col);
			//IgnoreAllCollisionExcept(true, 31, 9, 10);
			col.isTrigger = true;

			yield return 0f;

			if (path.Count != 0) { anim.SetBool("Walking", true); }
            MaxSpeed *= Stats.MoveSpeed;

			float _spd = StartingSpeedPercentage * MaxSpeed;
			int index=0;

			while (this) {
				if (index >= path.Count) { break; }

				if (!CanWalkAt(path[index])) {
					yield return 0f;
					//RequestPath.GetPath(rb.position, Position, path, Resolution.High);
					RequestPath.GetPath_Avoidance(rb.position, Position, path, Resolution.High, col);
					index = 0;
					continue;
				}

				var targetPos = grid.PosFromNode(path[index]);
				var offset = targetPos - rb.position;

				if (_spd != MaxSpeed) { _spd = Mathf.MoveTowards(_spd, MaxSpeed, 10 * MaxSpeed * Time.fixedDeltaTime); }
				float currentSpeed = _spd * Time.fixedDeltaTime;

				var posAfterMovement = Vector2.MoveTowards(rb.position, targetPos, currentSpeed);

				if (posAfterMovement == targetPos) {
					index++;

					if (index >= path.Count) { }
					else {
						targetPos = grid.PosFromNode(path[index]);
						offset = targetPos - rb.position;
						posAfterMovement = Vector2.MoveTowards(rb.position, targetPos, currentSpeed);
					}
				}

				rb.MovePosition(posAfterMovement);

				if (index >= path.Count) { break; }

				CalculateFacing(offset);

				yield return 0f;
			}

			if (!this) { yield break; }

			anim.SetBool("Walking", false);
			yield return 0f;

			GameLibOfMethods.player.GetComponent<Collider2D>().isTrigger = false;
			PlayerAnimationHelper.ResetPlayer();
			currentHandle = null;

			if (Callback != null) { Callback(); }
		}


		void CalculateFacing(Vector2 offset) {
			//bool isYBigger = Mathf.Abs(offset.x) <= 0.01f;
			bool isYBigger = Mathf.Abs(offset.x) < Mathf.Abs(offset.y);

			if (isYBigger) {
				if (offset.y > 0) { spr.FaceUP(); }
				else if (offset.y < 0) { spr.FaceDOWN(); }

				if (offset.y != 0) { anim.SetFloat("Vertical", Mathf.Sign(offset.y)); }
				else { anim.SetFloat("Vertical", 0); }

				anim.SetFloat("Horizontal", 0);
			}
			else {
				if (offset.x > 0) { spr.FaceRIGHT(); }
				else if (offset.x < 0) { spr.FaceLEFT(); }

				if (offset.x != 0) { anim.SetFloat("Horizontal", Mathf.Sign(offset.x)); }
				else { anim.SetFloat("Horizontal", 0); }

				anim.SetFloat("Vertical", 0);
			}
		}
		#endregion

		bool CanWalkAt(Node node) => node.isCurrentlyOccupied == null || node.isCurrentlyOccupied == col;
	}
}