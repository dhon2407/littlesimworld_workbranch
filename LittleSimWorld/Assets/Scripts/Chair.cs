using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : BreakableFurniture, IInteractable, IUseable {
    public Canvas LaptopOptionsCanvas;
    public bool IsFacingDown;

	public float InteractionRange { get; }
	public Vector3 PlayerStandPosition => CharacterPosition.position;

	public float CustomSpeedToPosition { get; }

	public void Interact() => PlayerCommands.MoveTo(this);

	public void Use() {
		StaySitting().Start();
		ActivateChoices();
	}

	public void ActivateChoices() => LaptopOptionsCanvas.gameObject.SetActive(true);
	public void DisableChoices() => LaptopOptionsCanvas.gameObject.SetActive(false);

	private IEnumerator<float> StaySitting() {
		GameLibOfMethods.sitting = true;
		GameLibOfMethods.cantMove = true;
		SpriteControler.Instance.FaceUP();

		ActivateChoices();

		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut) {
			yield return 0f;
		}

		PlayExitSound();

		GameLibOfMethods.sitting = false;
		yield return 0f;

		DisableChoices();
		PlayerCommands.WalkBackToLastPosition();

	}

	public void StartBrowsingInternet() {
		StartCoroutine(BrowsingInternet());
	}
	private IEnumerator BrowsingInternet() {

		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.animator.SetBool("Sitting", true);
		GameLibOfMethods.doingSomething = true;

		float moodDrainSpeed = MoodDrainPerHour;
		float energyDrainSpeed = EnergyDrainPerHour;
		float xpGainSpeed = XpGainGetHour;
		yield return new WaitForEndOfFrame();

		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !PlayerStatsManager.Instance.passingOut) {

			GameLibOfMethods.animator.SetBool("Learning", true);
			float multi = (Time.deltaTime / DayNightCycle.Instance.speed) * DayNightCycle.Instance.currentTimeSpeedMultiplier;
			PlayerStatsManager.Intelligence.Instance.AddXP(xpGainSpeed * multi);
			PlayerStatsManager.Instance.Energy -= (energyDrainSpeed * multi);
			PlayerStatsManager.Instance.Mood -= (moodDrainSpeed * multi);

			/*GameLibOfMethods.player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

			//blackScreen.CrossFadeAlpha(0, 1, false);

			GameLibOfMethods.facingDir = Vector2.left;*/

			yield return new WaitForEndOfFrame();
		}

		PlayExitSound();


		GameLibOfMethods.animator.SetBool("Learning", false);
		yield return new WaitForEndOfFrame();

		PlayerCommands.WalkBackToLastPosition();

		//Debug.Log("cant browse, busy doing something else");
	}

}
