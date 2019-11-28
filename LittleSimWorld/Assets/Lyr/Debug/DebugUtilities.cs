using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUtilities : MonoBehaviour
{
#if UNITY_EDITOR
	public Vector3 HomePos;
	public Vector3 GroceriesPos;

	Transform player;
	void Awake() {
		player = GameObject.Find("Player").transform;
		//FindObjectOfType<_CookingStove>().TimeToCook = 1;
	}
	void Update() {
		if (Input.GetKeyDown(KeyCode.Keypad0)) { player.position = HomePos; }
		if (Input.GetKeyDown(KeyCode.Keypad1)) { player.position = GroceriesPos; }
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(HomePos, 0.2f);
		Gizmos.DrawWireSphere(GroceriesPos, 0.2f);
	}
	#endif
}
