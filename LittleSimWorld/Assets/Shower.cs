using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

public class Shower : BreakableFurniture, IUseable, IInteractable {
	public Vector3 PlayerStandPosition => CharacterPosition.position;
	public float InteractionRange { get; }
	public float CustomSpeedToPosition => 3;

	public float HygieneGainAmount = 30;

	// Start is called before the first frame update
	protected override void Start() {
		base.Start();
		if (LoopingParticleSystem) {
			Emission = LoopingParticleSystem.emission;
			Emission.enabled = false;
			SoundSource.Stop();
		}
	}


	public void Use() => StartCoroutine(TakingShower());
	public void Interact() {
		SpriteControler.Instance.ChangeSortingOrder(7);
		PlayerCommands.JumpTo(this);
	}

	public IEnumerator TakingShower() {
		if (!GameLibOfMethods.canInteract) {

			GameLibOfMethods.cantMove = true;
			GameLibOfMethods.Walking = false;

			GameLibOfMethods.animator.SetBool("Jumping", false);
			GameLibOfMethods.animator.SetBool("TakingShower", true);

			Emission.enabled = true;
			yield return new WaitForEndOfFrame();
			SpriteControler.Instance.FaceDOWN();

			GameLibOfMethods.canInteract = true;
			yield return new WaitForEndOfFrame();

			float timeWithFullBar = 0;

			while (!Input.GetKey(InteractionChecker.Instance.KeyToInteract) && !GameLibOfMethods.passedOut && !isBroken) {


                Stats.Status(Type.Hygiene).Add(HygieneGainAmount * Time.fixedDeltaTime);

				float chance = Random.Range(0f, 100f);
				if (chance <= breakChancePerSecond / 60) {
					isBroken = true;
					Break();
				}


				if (Stats.Status(Type.Hygiene).CurrentAmount >= Stats.Status(Type.Hygiene).MaxAmount) {
					timeWithFullBar += Time.deltaTime;

					if (timeWithFullBar >= 2) {
						GameLibOfMethods.CreateFloatingText("You are too clean for this.", 2);
						break;
					}
				}
				yield return new WaitForFixedUpdate();
			}

			Emission.enabled = false;
			yield return new WaitForEndOfFrame();

			PlayExitSound();


			GameLibOfMethods.animator.SetBool("TakingShower", false);
			yield return new WaitForEndOfFrame();

			void act() => SpriteControler.Instance.ChangeSortingOrder(6);

			PlayerCommands.JumpOff(0, act);
		}
	}

}
