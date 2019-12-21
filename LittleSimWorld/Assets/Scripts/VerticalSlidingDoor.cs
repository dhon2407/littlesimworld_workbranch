using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClock = GameTime.Clock;

public class VerticalSlidingDoor : SlidingDoor
{
    protected override void Start()
    {
        startLeftPosition = leftDoor.transform.position;
        startRightPosition = rightDoor.transform.position;

        endLeftPosition = new Vector2(leftDoor.position.x , leftDoor.position.y - maxDoorPosition);
        endRightPosition = new Vector2(rightDoor.position.x , rightDoor.position.y + maxDoorPosition);

    }

	protected override void OpenDoor()
    {
        leftDoor.position = Vector2.Lerp(leftDoor.position, endLeftPosition, doorSpeed);
        rightDoor.position = Vector2.Lerp(rightDoor.position, endRightPosition, doorSpeed);
    }

    protected override void CloseDoor()
    {
        leftDoor.position = Vector2.Lerp(leftDoor.position, startLeftPosition, doorSpeed);
        rightDoor.position = Vector2.Lerp(rightDoor.position, startRightPosition, doorSpeed);
    }

}
