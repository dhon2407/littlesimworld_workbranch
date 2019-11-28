using UnityEngine;

public interface IUseable 
{
	Vector3 PlayerStandPosition { get; }
	float CustomSpeedToPosition { get; }

	void Use();
}
