using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkoutPlace : BreakableFurniture, IInteractable, IUseable {
    public GameObject Weights;

	public float InteractionRange => 1;
	public Vector3 PlayerStandPosition => Weights.transform.position;
	public float CustomSpeedToPosition { get; }

	public void Interact() => PlayerCommands.JumpTo(this);
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
		yield return new WaitForEndOfFrame();

		SpriteControler.Instance.FaceRIGHT();

		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut) {


			PlayerStatsManager.Strength.Instance.AddXP(0.027777778f);
			PlayerStatsManager.Instance.SubstractEnergy(0.027777778f);

			yield return new WaitForFixedUpdate();
		}

		Debug.Log("Playing exit sound");
		PlayExitSound();

		GameLibOfMethods.animator.SetBool("Lifting", false);
		yield return new WaitForEndOfFrame();

		PlayerCommands.JumpOff();

	}


}
