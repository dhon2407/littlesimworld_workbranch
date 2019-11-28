using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelephoneBooth : MonoBehaviour, IUseable, IInteractable {
	public float InteractionRange { get; }
	public Vector3 PlayerStandPosition { get; }
	public float CustomSpeedToPosition { get; }

	public void Interact() {
		Debug.Log("Using the booth");
		if (JobsPopUp.CurrentJob != null) {
			/*JobsPopUp.Instance.anim.SetBool("CarCalled", true);
			GameLibOfMethods.AddChatMessege("Called car to work.");*/
		}
	}
	public void Use() => Debug.Log("Using the booth");


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
