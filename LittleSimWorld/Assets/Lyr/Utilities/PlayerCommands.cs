using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public static class PlayerCommands {
	static float DefaultJumpSpeed => InteractionChecker.Instance.JumpSpeed;

	static AnimationCurve jumpCurve => InteractionChecker.Instance.jumpCurve;

	public static Vector3 LastPositionBeforeJump;
	public static Vector3 LastPositionBeforeWalk;

	public static void MoveTo(Vector3 position, Action callback) {
		StartWalking(position, callback).Start();
	}
	public static void MoveTo(IUseable useable, Action callback = null) {
		Action action = callback ?? (() => useable.Use());
		StartWalking(useable.PlayerStandPosition, action).Start();
	}

	public static void JumpTo(Vector3 position, Action callback) {
		StartJumping(position, callback).Start();
	}
	public static void JumpTo(IUseable useable, Action callback = null) {
		var action = callback ?? (() => useable.Use());
		StartJumping(useable.PlayerStandPosition, action, useable, useable.CustomSpeedToPosition).Start();
	}

	public static void WaitUntilAnimationFinished(string animation, Action callback) => WaitUntilAnimationFinishes(animation).AddCallback(callback).Start();

	public static void JumpOff(float CustomSpeed = 0, Action CustomCallBack = null) {
		if (CustomCallBack == null) { CustomCallBack = PlayerAnimationHelper.ResetPlayer; }
		else { CustomCallBack = PlayerAnimationHelper.ResetPlayer + CustomCallBack; }
		StartJumping(LastPositionBeforeJump, CustomCallBack, null, CustomSpeed).Start();
	}
	public static void WalkBackToLastPosition() => StartWalking(LastPositionBeforeWalk, PlayerAnimationHelper.ResetPlayer).Start();

	public static void DelayAction(float waitTime, Action callback = null) => DelayWithAction(waitTime).AddCallback(callback).Start();
	public static void DelayAction(float waitTime, Action callback, Func<bool> cancelCondition, Action cancelCallback = null) => DelayWithAction(waitTime).AddCallback(callback).CancelIf(cancelCondition, cancelCallback).Start();
	public static void FinishAnimation() { StartFinishAnimation().Start(Segment.FixedUpdate); }

	static IEnumerator<float> StartJumping(Vector3 TargetPosition, Action callback, IUseable useable = null, float CustomSpeed = 0) {

		var StartPosition = GameLibOfMethods.player.transform.position;

		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.doingSomething = true;

		Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);

		GameLibOfMethods.TempPos = GameLibOfMethods.player.transform.position;
		GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		GameLibOfMethods.animator.SetBool("Jumping", true);

		float T = 0;
		float _speed = CustomSpeed > 0 ? CustomSpeed : DefaultJumpSpeed;


		while (true) {
			T += _speed * Time.deltaTime;

			GameLibOfMethods.player.transform.position = Vector2.Lerp(StartPosition, TargetPosition, T);
			GameLibOfMethods.player.transform.localScale = new Vector3(jumpCurve.Evaluate(T), jumpCurve.Evaluate(T), 1);

			// if our value has reached the total, break out of the loop
			if (T >= 1) { break; }

			yield return 0f;
		}

		//PlayerAnimationHelper.ResetPlayer();
		callback?.Invoke();
		LastPositionBeforeJump = StartPosition;
	}

	static IEnumerator<float> StartWalking(Vector2 TargetPosition, Action callback) {

		var StartPosition = GameLibOfMethods.player.transform.position;

		PlayerAnimationHelper.ResetAnimations();

		GameLibOfMethods.doingSomething = true;
		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.cantMove = true;

		GameLibOfMethods.animator.SetBool("Walking", true);

		float T = 0;

		Vector2 temp = GameLibOfMethods.player.transform.position;
		while (true) {
			T += 0.04f * Time.deltaTime / Time.fixedDeltaTime;
			if (Mathf.Abs((TargetPosition - temp).normalized.x) < Mathf.Abs((TargetPosition - temp).normalized.y)) {
				GameLibOfMethods.animator.SetFloat("Vertical", (TargetPosition - temp).normalized.y);
			}
			else {
				GameLibOfMethods.animator.SetFloat("Horizontal", (TargetPosition - temp).normalized.x);
			}

			GameLibOfMethods.animator.SetBool("Walking", true);
			GameLibOfMethods.player.transform.position = Vector2.Lerp(temp, TargetPosition, T);

			PlayerAnimationHelper.HandlePlayerFacing();

			// if our value has reached the total, break out of the loop
			if (T >= 1) { break; }

			yield return 0f;
		}

		GameLibOfMethods.animator.SetBool("Walking", false);

		callback?.Invoke();
		LastPositionBeforeWalk = StartPosition;
	}
	static IEnumerator<float> StartFinishAnimation() {
		float T = 0;

		var renderers = GameLibOfMethods.player.GetComponentsInChildren<SpriteRenderer>();

		while (true) {
			T += 0.02f;

			foreach (SpriteRenderer spr in renderers) {
				float a = Mathf.SmoothStep(1, 0, T);
				spr.color = new Color(1f, 1f, 1f, a);
			}

			if (T >= 1f) { break; }

			yield return 0f;
		}

		T = 0;

		while (true) {
			T += 0.02f;

			foreach (SpriteRenderer sprite in renderers) {
				float a = Mathf.SmoothStep(0, 1, T);
				sprite.color = new Color(1f, 1f, 1f, a);
			}
			if (T >= 1f) { break; }

			yield return 0f;
		}
	}

	static IEnumerator<float> WaitUntilAnimationFinishes(string animation) {
		var anim = GameLibOfMethods.animator;
		anim.Play(animation);
		yield return 0f;

		while (true) {
			if (!anim.GetCurrentAnimatorStateInfo(0).IsName(animation)) { break; }
			yield return 0f;
		}
	}


	static IEnumerator<float> DelayWithAction(float waitTime) {
		float t = 0;
		while (true) {
			t += Time.deltaTime;
			if (t >= waitTime) { break; }
			yield return 0f;
		}
	}
}