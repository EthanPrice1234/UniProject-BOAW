using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsJumping : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // This doesn't seem to be working even when adding a fixed state exit time ???
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isJumping", false);
        // animator.SetBool("isGrounded", false);
    }

}
