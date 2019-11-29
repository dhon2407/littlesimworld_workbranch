using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassOutState : StateMachineBehaviour {

	public bool CheckOutReason = false;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		GameLibOfMethods.doingSomething = true;
		GameLibOfMethods.canInteract = false;
		GameLibOfMethods.cantMove = true;

		if (CheckOutReason) { CheckPassOutReason(animator, stateInfo, layerIndex); }
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		animator.SetBool("PassOut", false);
	}

	void CheckPassOutReason(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (PlayerStatsManager.GetCurrentAmount(StatusBarType.Energy) <= 0) { animator.SetBool("Sleeping", true); }
		else if (PlayerStatsManager.GetCurrentAmount(StatusBarType.Health) <= 0) { WakeUpHospital().Start(); }
	}


	public static IEnumerator<float> WakeUpHospital() {
		PlayerStatsManager.Instance.passingOut = false;
        GameTime.Clock.ResetSpeed();
		GameLibOfMethods.blackScreen.CrossFadeAlpha(1, 0.5f, false);
		yield return MEC.Timing.WaitForSeconds(2);

		var player = GameLibOfMethods.player;

		player.GetComponent<Animator>().enabled = true;


		var keys = PlayerStatsManager.Instance.playerStatusBars.Keys;

		foreach (var key in keys) {
			PlayerStatsManager.PlayerStatusBars[key].CurrentAmount = 100;
		}
		// can be in stead of 

		player.transform.rotation = Quaternion.Euler(0,0,0);

		PlayerStatsManager.Instance.SubstractMoney(GameLibOfMethods.HospitalFee);

		GameLibOfMethods.passedOut = false;
		Vector3 SpawnPosition = GameLibOfMethods.HospitalRespawnPoint.position;
		SpawnPosition.z = player.transform.position.z;

		player.transform.position = SpawnPosition;

		GameLibOfMethods.blackScreen.CrossFadeAlpha(0, 2, false);
		GameLibOfMethods.cantMove = false;
		CameraFollow.Instance.ResetCamera();

		PlayerAnimationHelper.ResetPlayer();
	}
}
