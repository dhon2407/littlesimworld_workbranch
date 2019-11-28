using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Bed : BreakableFurniture, IInteractable, IUseable {
	public float InteractionRange => 1;
	public Vector3 PlayerStandPosition => CharacterPosition.position;

	[ShowInInspector]
	public float CustomSpeedToPosition { get; set; } = 5;

	public float JumpOffSpeed = 5;

	bool CanPlayerSleep => !GameLibOfMethods.doingSomething && PlayerStatsManager.Instance.Food > PlayerStatsManager.Instance.MaxFood * 0.1f &&
												PlayerStatsManager.Instance.Thirst > PlayerStatsManager.Instance.MaxThirst * 0.1f &&
												PlayerStatsManager.Instance.Bladder > PlayerStatsManager.Instance.MaxBladder * 0.1f &&
												PlayerStatsManager.Instance.Hygiene > PlayerStatsManager.Instance.MaxHygiene * 0.1f;
	public void Interact() {
		if (!CanPlayerSleep) {
			Debug.Log("Player cannot sleep at the moment");
			return;
		}

		JumpToBed().Start();
	}

	public void Use() => Sleeping().Start();


	IEnumerator<float> JumpToBed() {
		var anim = GameLibOfMethods.animator;
		anim.Play("PrepareToSleep");
		GameLibOfMethods.animator.SetBool("Sleeping", true);
		yield return 0f;

		PlayerCommands.LastPositionBeforeJump = GameLibOfMethods.player.transform.position;

		while (true) {
			if (!anim.GetCurrentAnimatorStateInfo(0).IsName("PrepareToSleep")) {
				Debug.Log("Something is wrong.. this finished before firing the method. Make sure you don't call this from FixedUpdate.");
				break;
			}

			if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 80 / 120f) { break; }

			yield return 0f;
		}

		Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);
		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.doingSomething = true;

		while (true) {
			GameLibOfMethods.player.transform.position = Vector2.MoveTowards(GameLibOfMethods.player.transform.position, CharacterPosition.position, 3 * Time.deltaTime);
			if (GameLibOfMethods.player.transform.position == CharacterPosition.position) { break; }
			yield return 0f;
		}

		yield return 0f;
		Use();
	}

	IEnumerator<float> Sleeping() {

		//PlayerAnimationHelper.ResetPlayer();
		yield return 0f;
		Debug.Log("Went to sleep");

		Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, true);
		GameLibOfMethods.isSleeping = true;
		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.animator.SetBool("Sleeping", true);
		GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		GameLibOfMethods.AddChatMessege("Went to sleep.");

		yield return 0f;


		float T = 0;
		DayNightCycle.Instance.ChangeSpeedToSleepingSpeed();
		while (true) {
			GameLibOfMethods.concecutiveSleepTime += (Time.deltaTime * DayNightCycle.Instance.currentTimeSpeedMultiplier) * DayNightCycle.Instance.speed;
			float Multi = (Time.deltaTime / DayNightCycle.Instance.speed) * DayNightCycle.Instance.currentTimeSpeedMultiplier;

			PlayerStatsManager.Instance.AddEnergy(EnergyGainPerHour * Multi);
			PlayerStatsManager.Instance.AddMood(MoodGainPerHour * Multi);
			PlayerStatsManager.Instance.Heal(HealthGainPerHour * Multi);

			if (Input.GetKeyUp(KeyCode.E)) { break; }

			if (PlayerStatsManager.Instance.Energy >= PlayerStatsManager.Instance.MaxEnergy) {
				T += Time.deltaTime;

				if (T >= 2) {
					GameLibOfMethods.CreateFloatingText("Can't sleep more", 2);
					break;
				}
			}
			yield return 0f;
		}

		PlayExitSound();

		GameLibOfMethods.animator.SetBool("Sleeping", false);
		yield return 0f;

		while (true) {
			var state = GameLibOfMethods.animator.GetCurrentAnimatorStateInfo(0);
			if (!state.IsName("JumpOffToBed")) {
				Debug.Log("Something is wrong.. this finished before firing the method. Make sure you don't call this from FixedUpdate.");
				break;
			}

			if (state.normalizedTime >= 40 / 50f) {
				break;
			}
			yield return 0f;
		}
		PlayerCommands.JumpOff(JumpOffSpeed);

		DayNightCycle.Instance.ChangeSpeedToNormal();

		//Debug.Log("Saving not implemented");
		GameManager.Instance.SaveGame();
	}

}