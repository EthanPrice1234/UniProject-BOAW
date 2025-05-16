using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrawn : StateMachineBehaviour
{
    private PlayerCombat playerCombat;

    void Awake()
    {
        playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerCombat.OnWeaponDrawn();
    }
}

