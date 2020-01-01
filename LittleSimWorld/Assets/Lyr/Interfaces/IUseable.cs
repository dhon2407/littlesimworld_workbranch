using UnityEngine;

public interface IUseable 
{
	Vector2 PlayerStandPosition { get; }
	float CustomSpeedToPosition { get; }

	void Use();
}
