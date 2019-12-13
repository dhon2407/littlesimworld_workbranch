using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUtilities : MonoBehaviour
{
#if UNITY_EDITOR
	public Vector3 HomePos;
	public Vector3 GroceriesPos;
	public Vector3 TestViewPos;

	Transform player => GameLibOfMethods.player.transform;

	bool Cheated3;

	void Update() {
		if (Input.GetKeyDown(KeyCode.Keypad0)) { player.position = HomePos; }
		if (Input.GetKeyDown(KeyCode.Keypad1)) { player.position = GroceriesPos; }
		if (Input.GetKeyDown(KeyCode.Keypad2)) { PlayerStatsManager.Instance.Money += 100f; }
		if (Input.GetKeyDown(KeyCode.Keypad9)) { player.position = TestViewPos; }
		if (Input.GetKeyDown(KeyCode.Keypad3)) {
			_CookingStove.instance.DefaultCookingEXP = Cheated3 ? 5 : 100;
			_CookingStove.instance.TimeToCook = Cheated3 ? 10 : 5;

			Cheated3 = !Cheated3;
		}
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(HomePos, 0.2f);
		Gizmos.DrawWireSphere(GroceriesPos, 0.2f);
		Gizmos.DrawWireSphere(TestViewPos, 0.2f);
	}
#endif
}
