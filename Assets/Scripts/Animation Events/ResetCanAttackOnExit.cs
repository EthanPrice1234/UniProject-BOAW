using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCanAttack : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat playerCombat = animator.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.ResetCanAttack();
        }
    }
}
