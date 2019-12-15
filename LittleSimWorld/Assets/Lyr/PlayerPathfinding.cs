using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPathfinding : MonoBehaviour {

	Animator anim;
	Rigidbody2D rb;
	SpriteControler spr;
	GameObject player;

	public float Speed = 2.65f;

	void Start() {
		player = GameLibOfMethods.player;
		spr = FindObjectOfType<SpriteControler>();
		anim = GameLibOfMethods.animator;
		rb = player.GetComponent<Rigidbody2D>();

		contactFilter = new ContactFilter2D();
		contactFilter.SetLayerMask(GameLibOfMethods.Instance.InteractablesLayer);
	}


	


	void Update() {

		bool shouldCancel = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && currentHandle.HasValue;
		if (shouldCancel) { Cancel(); }

		if (Input.GetMouseButtonDown(0)) {
			var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var interactable = CheckInteractable(pos);

			if (interactable != null) {
				if (currentHandle.HasValue) { Cancel(); }
				MoveTo(interactable.PlayerStandPosition, Speed, interactable.Interact);
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			if (currentHandle.HasValue) { Cancel(); }
			var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			MoveTo(pos, Speed, null);
		}
	}

	void Cancel() {
		MEC.Timing.KillCoroutines(currentHandle.Value);
		currentHandle = null;
		anim.SetBool("Walking", false);
		GameLibOfMethods.player.GetComponent<Collider2D>().enabled = true;
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


	#region Movement
	List<Node> path = new List<Node>(1000);
	ContactFilter2D contactFilter = new ContactFilter2D();
	MEC.CoroutineHandle? currentHandle;
	public void MoveTo(Vector3 pos, float speed, System.Action callback) {
		if (currentHandle.HasValue) { Cancel(); }
		if (GameLibOfMethods.doingSomething) {
			Debug.Log("Doing Something");
			return;
		}
		currentHandle = WalkTo(pos, speed, callback).Start(MEC.Segment.FixedUpdate);
	}


	IEnumerator<float> WalkTo(Vector3 Position, float Speed, System.Action Callback) {

		GameLibOfMethods.player.GetComponent<Collider2D>().enabled = false;
		PlayerAnimationHelper.StopPlayer();
		RequestPath.GetPath(rb.position, Position, path);
		if (path.Count == 0) { yield break; }
		yield return 0f;
		anim.SetBool("Walking", true);
		Speed *= PlayerStatsManager.Instance.MovementSpeed;
		float _spd = 0;
		int index = 0;

		while (this) {

			float spd = _spd * Time.fixedDeltaTime;
			if (_spd != Speed) { _spd = Mathf.MoveTowards(_spd, Speed, 10 * Speed * Time.fixedDeltaTime); }

			if (index >= path.Count) {
				Debug.Log($"count: {path.Count} index: {index} error getting correct pos");
				break;
			}

			var targetPos = path[index].worldPosition;
			var offset = path[index].worldPosition - rb.position;

			var posAfterMovement = Vector2.MoveTowards(rb.position, targetPos, spd);

			if (posAfterMovement == targetPos) {
				index++;

				if (index >= path.Count) { }
				else {
					targetPos = path[index].worldPosition;
					offset = path[index].worldPosition - rb.position;
					posAfterMovement = Vector2.MoveTowards(rb.position, targetPos, spd);
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

		GameLibOfMethods.player.GetComponent<Collider2D>().enabled = true;
		PlayerAnimationHelper.ResetPlayer();
		currentHandle = null;

		if (Callback != null) { Callback(); }
	}

	
	void CalculateFacing(Vector2 offset) {
		bool isYBigger = Mathf.Abs(offset.x) <= 0.01f;//Mathf.Abs(offset.x) < Mathf.Abs(offset.y);

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
}
