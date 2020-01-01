using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStats;
using GameClock = GameTime.Clock;
using static PlayerStats.Skill;
using Stats = PlayerStats.Stats;

public class Chair : BreakableFurniture, IInteractable, IUseable {
    public Canvas LaptopOptionsCanvas;
    public bool IsFacingDown;

	public float InteractionRange { get; }
	public Vector2 PlayerStandPosition => CharacterPosition.position;

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

		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !GameLibOfMethods.passedOut) {
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
    public void StartPracticingWriting()
    {
        StartCoroutine(PracticeWriting());
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

		while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !GameLibOfMethods.passedOut) {

			GameLibOfMethods.animator.SetBool("Learning", true);
			float multi = (Time.deltaTime / GameClock.Speed) * GameClock.TimeMultiplier;
            Stats.AddXP(Skill.Type.Intelligence, xpGainSpeed * multi);
            Stats.Status(Status.Type.Energy).Remove(energyDrainSpeed * multi);
            Stats.Status(Status.Type.Mood).Remove(energyDrainSpeed * multi);

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
    private IEnumerator PracticeWriting()
    {

        GameLibOfMethods.cantMove = true;
        GameLibOfMethods.canInteract = false;
        GameLibOfMethods.animator.SetBool("Sitting", true);
        GameLibOfMethods.doingSomething = true;

        float moodDrainSpeed = MoodDrainPerHour;
        float energyDrainSpeed = EnergyDrainPerHour;
        float xpGainSpeed = XpGainGetHour;
        yield return new WaitForEndOfFrame();

        while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !GameLibOfMethods.passedOut)
        {

            GameLibOfMethods.animator.SetBool("Learning", true);
            float multi = (Time.deltaTime / GameClock.Speed) * GameClock.TimeMultiplier;

            Stats.AddXP(Type.Writing, xpGainSpeed * multi);
            Stats.Remove(Status.Type.Energy, energyDrainSpeed * multi);
            Stats.Remove(Status.Type.Mood, moodDrainSpeed * multi);

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
