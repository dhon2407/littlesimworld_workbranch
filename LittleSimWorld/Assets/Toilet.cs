using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : BreakableFurniture, IUseable, IInteractable {
	public float InteractionRange => 1;
	public Vector3 PlayerStandPosition => CharacterPosition.position;

	public float CustomSpeedToPosition => 3;

	public float BladderGainAmount = 30;

	public void Interact() {
		SpriteControler.Instance.ChangeSortingOrder(7);
		PlayerCommands.JumpTo(this);
	}
	public void Use() => StartCoroutine(UseToilet());

	IEnumerator UseToilet() {

		GameLibOfMethods.cantMove = true;
		SpriteControler.Instance.FaceLEFT();
		GameLibOfMethods.animator.SetBool("Jumping", false);
		GameLibOfMethods.animator.SetBool("TakingADump", true);

		yield return new WaitForFixedUpdate();

		SpriteControler.Instance.FaceLEFT();

		float timeWithFullBar = 0;
		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut) {



			PlayerStatsManager.Bladder.Instance.Add(BladderGainAmount * Time.fixedDeltaTime);


			if (PlayerStatsManager.Bladder.Instance.CurrentAmount >= PlayerStatsManager.Bladder.Instance.MaxAmount) {
				timeWithFullBar += Time.deltaTime;

				if (timeWithFullBar >= 2) {
					GameLibOfMethods.CreateFloatingText("You have no juice left in your body.", 2);
					break;
				}
			}

			yield return new WaitForFixedUpdate();
		}

		PlayExitSound();

		GameLibOfMethods.animator.SetBool("TakingADump", false);
		yield return new WaitForEndOfFrame();

		void act() => SpriteControler.Instance.ChangeSortingOrder(6);

		PlayerCommands.JumpOff(0, act);
	}

}
