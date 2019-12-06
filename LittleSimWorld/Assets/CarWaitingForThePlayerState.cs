using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWaitingForThePlayerState : StateMachineBehaviour
{
    public float waitingTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        JobCar.Instance.CarReadyToInteract = true;
    }

   
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        waitingTime += Time.deltaTime;

        if(waitingTime > JobManager.Instance.CurrentJob.MaxCarWaitTime)
        {
            JobCar.Instance.CarDriveFromHouseToLeft();
            waitingTime = 0;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        JobCar.Instance.CarReadyToInteract = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
