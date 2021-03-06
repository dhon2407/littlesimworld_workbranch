﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerAnimationHelper
{

	public static void ResetAnimations() {
		GameLibOfMethods.animator.SetBool("Lifting", false);
		GameLibOfMethods.animator.SetBool("TakingADump", false);
		GameLibOfMethods.animator.SetBool("TakingShower", false);
		GameLibOfMethods.animator.SetBool("Cooking", false);
		GameLibOfMethods.animator.SetBool("Mixing", false);
		GameLibOfMethods.animator.SetBool("Sleeping", false);
		GameLibOfMethods.animator.SetBool("Jumping", false);
		GameLibOfMethods.animator.SetBool("Eating", false);
		GameLibOfMethods.animator.SetBool("Learning", false);
		//GameLibOfMethods.animator.SetBool("Sitting", false);
		GameLibOfMethods.animator.SetBool("Drinking", false);
		GameLibOfMethods.animator.SetBool("Fixing", false);
		GameLibOfMethods.animator.ResetTrigger("PassOutToSleep");
        GameLibOfMethods.animator.SetBool("HidePlayer", false);
    }

	public static void ResetPlayer() {
		Physics2D.IgnoreLayerCollision(GameLibOfMethods.player.layer, 10, false);

		GameLibOfMethods.sitting = false;
		GameLibOfMethods.isSleeping = false;
		GameLibOfMethods.cantMove = false;
		GameLibOfMethods.canInteract = true;
		GameLibOfMethods.doingSomething = false;
        GameLibOfMethods.passedOut = false;

		GameLibOfMethods.player.transform.rotation = Quaternion.Euler(Vector2.zero);
		GameLibOfMethods.animator.enabled = true;

        GameTime.Clock.ResetSpeed();

        SpriteControler.Instance.LeftHand.position = SpriteControler.Instance.LeftHandLeft.transform.position;
		SpriteControler.Instance.LeftHand.GetComponent<SpriteRenderer>().sortingOrder = 0;
		SpriteControler.Instance.RightHand.position = SpriteControler.Instance.RightHandRight.transform.position;
		SpriteControler.Instance.RightHand.GetComponent<SpriteRenderer>().sortingOrder = 0;

        JobManager.Instance.isWorking = false;

        ResetAnimations();
	}

	public static void StopPlayer() {
		GameLibOfMethods.cantMove = true;
		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.doingSomething = true;
	}

	public static void HandlePlayerFacing() {
		if (GameLibOfMethods.animator.GetFloat("Vertical") < 0) {
			SpriteControler.Instance.FaceDOWN();
		}
		if (GameLibOfMethods.animator.GetFloat("Vertical") > 0) {
			SpriteControler.Instance.FaceUP();
		}
		if (GameLibOfMethods.animator.GetFloat("Horizontal") < 0) {
			SpriteControler.Instance.FaceLEFT();
		}
		if (GameLibOfMethods.animator.GetFloat("Horizontal") > 0) {
			SpriteControler.Instance.FaceRIGHT();
		}
	}


}
