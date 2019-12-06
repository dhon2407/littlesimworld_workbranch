using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PissingInPantsState : StateMachineBehaviour
{
   
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerStatsManager. Bladder.Instance.CurrentAmount = PlayerStatsManager.Bladder.Instance.MaxAmount;
        PlayerStatsManager.Hygiene.Instance.CurrentAmount = 0;
        GameLibOfMethods.cantMove = true;
        GameLibOfMethods.canInteract = false;
        GameLibOfMethods.doingSomething = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    
   override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAnimationHelper.ResetPlayer();
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
