using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorStateChanger : MonoBehaviour
{
    public Animator anim;
    public float walkingSpeed;
	Rigidbody2D rb;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();	
	}

	void FixedUpdate()
    {
		float speedMulti = walkingSpeed * PlayerStatsManager.Instance.MovementSpeed;
		Vector3 temp = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speedMulti;
        temp = Vector3.ClampMagnitude(temp, walkingSpeed);

		if (!GameLibOfMethods.cantMove && !GameLibOfMethods.sitting && !SpriteControler.Instance.BeignKnockbacked) {
			rb.velocity = temp;
			if (temp.sqrMagnitude != 0) { anim.SetBool("Walking", true); }
			else { anim.SetBool("Walking", false); }
		}
		else { anim.SetBool("Walking", false); }
        
    }
}
