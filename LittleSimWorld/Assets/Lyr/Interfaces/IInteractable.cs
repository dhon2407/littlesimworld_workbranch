
using UnityEngine;

public interface IInteractable 
{
	float InteractionRange { get; }
	Vector2 PlayerStandPosition { get; }
	GameObject gameObject { get; }

	void Interact();
}
