using System.Collections;
using UnityEngine;

using static PlayerStats.Status.Type;
using static PlayerStats.Skill.Type;
using Stats = PlayerStats.Stats;

public class WorkoutPlace : BreakableFurniture, IInteractable, IUseable {
    public GameObject Weights;

	public float InteractionRange => 1;
	public Vector3 PlayerStandPosition => CharacterPosition.position;
	public float CustomSpeedToPosition { get; }

	public void Interact() {
		if (Stats.Status(Energy).CurrentAmount <= 5 || Stats.Status(Health).CurrentAmount <= 5) { return; }
		PlayerCommands.JumpTo(this);
	}

	public void Use() {
		GameLibOfMethods.player.GetComponent<SpriteControler>().FaceRIGHT();
		Weights.SetActive(false);

		Weights.SetActive(true);

		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.Walking = false;

		StartCoroutine(StartLifting());
	}

	public IEnumerator StartLifting() {

		GameLibOfMethods.animator.SetBool("Jumping", false);
		GameLibOfMethods.animator.SetBool("Lifting", true);
		Weights.SetActive(false);

		yield return new WaitForEndOfFrame();
		SpriteControler.Instance.FaceRIGHT();

		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract)) {

            Stats.AddXP(Strength, 0.027777778f);
            Stats.Status(Energy).Remove(0.027777778f);

			if (Stats.Status(Energy).CurrentAmount <= 0 || Stats.Status(Health).CurrentAmount <= 0) { break; }

			yield return new WaitForFixedUpdate();
		}

		Debug.Log("Playing exit sound");
		PlayExitSound();

		GameLibOfMethods.animator.SetBool("Lifting", false);
		yield return new WaitForEndOfFrame();
		Weights.SetActive(true);


		if (Stats.Status(Energy).CurrentAmount <= 0 || Stats.Status(Health).CurrentAmount <= 0) {
			void act() => GameLibOfMethods.animator.SetBool("PassOut", true);
			PlayerCommands.JumpOff(0, act);
		}
		else { PlayerCommands.JumpOff(); }

	}


}
