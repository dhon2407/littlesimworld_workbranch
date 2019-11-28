using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : BreakableFurniture, IInteractable, IUseable {

	public float InteractionRange => 1;

	public Vector3 PlayerStandPosition => CharacterPosition.position;

	public float CustomSpeedToPosition { get; }

	public void Interact() => PlayerCommands.JumpTo(this);
	public void Use() => StartCoroutine(RunningOnTreadmill());

	public IEnumerator RunningOnTreadmill() {

		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.canInteract = true;

		SpriteControler.Instance.FaceLEFT();
		GameLibOfMethods.animator.SetBool("Jumping", false);
		GameLibOfMethods.animator.SetBool("Walking", true);

		yield return new WaitForFixedUpdate();

		SpriteControler.Instance.FaceLEFT();

		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut) {
			PlayerStatsManager.Fitness.Instance.AddXP(0.027777778f);
			PlayerStatsManager.Instance.SubstractEnergy(0.027777778f);
			GameLibOfMethods.animator.SetBool("Walking", true);

			yield return new WaitForFixedUpdate();

		}

		Debug.Log("Playing exit sound");
		PlayExitSound();


		GameLibOfMethods.animator.SetBool("Walking", false);
		yield return new WaitForEndOfFrame();

		PlayerCommands.JumpOff();
	}
}
