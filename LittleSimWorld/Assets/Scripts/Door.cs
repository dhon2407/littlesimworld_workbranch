using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {
	public bool isOpen;
	public Sprite OpenDoor;
	public Sprite ClosedDoor;

	public float InteractionRange => 1;

	public Vector2 PlayerStandPosition => transform.position;

	public void Interact() {
		if (!isOpen) {
			isOpen = true;
			GetComponent<Collider2D>().isTrigger = true;
			GetComponent<SpriteRenderer>().sprite = OpenDoor;
			return;
		}
		if (isOpen) {
			isOpen = false;
			GetComponent<Collider2D>().isTrigger = false;
			GetComponent<SpriteRenderer>().sprite = ClosedDoor;
			return;
		}
	}

}
