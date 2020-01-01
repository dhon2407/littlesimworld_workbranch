using System.Collections;
using UnityEngine;

using static PlayerStats.Status.Type;
using static PlayerStats.Skill.Type;
using Stats = PlayerStats.Stats;

public class Treadmill : BreakableFurniture, IInteractable, IUseable {

	public float InteractionRange => 1;

	public Vector2 PlayerStandPosition => CharacterPosition.position;

	public float CustomSpeedToPosition { get; }

	public void Interact() {
		if (Stats.Status(Energy).CurrentAmount <= 5 || Stats.Status(Health).CurrentAmount <= 5) { return; }
		PlayerCommands.JumpTo(this);
	}
	public void Use() => StartCoroutine(RunningOnTreadmill());

	public IEnumerator RunningOnTreadmill() {

		
		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.canInteract = true;

		SpriteControler.Instance.FaceLEFT();
		GameLibOfMethods.animator.SetBool("Jumping", false);
		GameLibOfMethods.animator.SetBool("Walking", true);
		GameLibOfMethods.animator.SetFloat("Horizontal", -1);

		yield return new WaitForFixedUpdate();


		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract)) {
			GameLibOfMethods.animator.SetBool("Walking", true);

			SpriteControler.Instance.FaceLEFT();

            Stats.AddXP(Fitness, 0.027777778f);
            Stats.Status(Energy).Remove(0.027777778f);

			if (Stats.Status(Energy).CurrentAmount <= 0 || Stats.Status(Health).CurrentAmount <= 0) { break; }


			yield return new WaitForFixedUpdate();

		}
		GameLibOfMethods.animator.SetFloat("Horizontal", 0);

		Debug.Log("Playing exit sound");
		PlayExitSound();

		GameLibOfMethods.animator.SetBool("Walking", false);
		yield return new WaitForEndOfFrame();


		if (Stats.Status(Energy).CurrentAmount <= 0 || Stats.Status(Health).CurrentAmount <= 0) {
			void act() => GameLibOfMethods.animator.SetBool("PassOut", true);
			PlayerCommands.JumpOff(0, act);
		}
		else { PlayerCommands.JumpOff(); }
	}
}
