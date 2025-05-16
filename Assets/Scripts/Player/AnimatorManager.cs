using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    PlayerCombat playerCombat;
    int horizontal;
    int vertical; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerCombat = GetComponent<PlayerCombat>(); 
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    // Play a target animatino by specific name
    public void PlayTargetAnimation(string targetAnimation, bool isInteracting) 
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    // Play a weapon specific animation 
    public void PlayWeaponSpecificAnimation(string action, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        string weaponName = playerCombat.GetCurrentWeaponName();
        string animationName = weaponName + " " + action;
        animator.CrossFade(animationName, 0.2f);
    }

    // Play a weapon specific animation by bound weapon name
    public void PlayBoundWeaponAnimation(string action, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        string boundWeaponName = playerCombat.GetBoundWeaponName();
        string animationName = boundWeaponName + " " + action;
        animator.CrossFade(animationName, 0.2f);
    }
    
    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement)
    {
        float snappedHorizontal;
        float snappedVertical; 

        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > 0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f) 
        {
            snappedVertical = -1;
        }
        else 
        {
            snappedVertical = 0;
        }
        #endregion 

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > 0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f) 
        {
            snappedHorizontal = -1;
        }
        else 
        {
            snappedHorizontal = 0;
        }
        #endregion 

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }
}