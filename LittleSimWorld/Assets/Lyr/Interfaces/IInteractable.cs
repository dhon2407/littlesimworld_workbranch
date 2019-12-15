
using UnityEngine;

public interface IInteractable 
{
	float InteractionRange { get; }
	Vector3 PlayerStandPosition { get; }
	GameObject gameObject { get; }

	void Interact();
}
