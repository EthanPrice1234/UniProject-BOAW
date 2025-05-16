using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerActions playerActions;
    PlayerLocomotion playerLocomotion;
    PlayerCombat playerCombat;
    AnimatorManager animatorManager;
    InteractionManager interactionManager;
    PlayerManager playerManager;

    public Vector2 movementInput;
    public Vector2 cameraInput; 

    public float cameraInputX;
    public float cameraInputY; 

    public float moveAmount; 
    public float verticalInput;
    public float horizontalInput;

    public bool sprintInput;
    public bool jumpInput;
    public bool rollInput; 
    public bool blockInput; 
    public bool attackInput;
    public bool interactInput;
    public bool drawWeaponInput;
    public bool switchWeaponInput;

    private float attackHoldTime = 0f;
    private bool attackHeld = false;
    private const float heavyAttackThreshold = 1.0f; // heavy charge time

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerCombat = GetComponent<PlayerCombat>();
        interactionManager = GetComponent<InteractionManager>();
        playerManager = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        if (playerActions == null)
        {
            playerActions = new PlayerActions();

            playerActions.Controls.Move.performed += i => movementInput = i.ReadValue<Vector2>();
            playerActions.Controls.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerActions.Controls.Sprint.performed += i => sprintInput = true; 
            playerActions.Controls.Sprint.canceled += i => sprintInput = false;  
            playerActions.Controls.Jump.performed += i => jumpInput = true;

            playerActions.Controls.Interact.performed += i => interactInput = true; 
            playerActions.Controls.Interact.canceled += i => interactInput = false;
            
            playerActions.Combat.Roll.performed += i => rollInput = true; 
            playerActions.Combat.Block.performed += i => blockInput = true;
            playerActions.Combat.Block.canceled += i => blockInput = false; 
            playerActions.Combat.Attack.performed += i => attackInput = true;
            playerActions.Combat.Attack.canceled += i => attackInput = false;

            // New input bindings for weapon system
            playerActions.Combat.DrawWeapon.performed += i => drawWeaponInput = true;
            playerActions.Combat.SwitchWeapon.performed += i => switchWeaponInput = true;
        }

        playerActions.Enable();
    }

    private void OnDisable()
    {
        playerActions.Disable();
    }

    public void HandleAllInputs() 
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleRollInput();
        HandleBlockInput();
        HandleAttackInput();
        HandleInteractionInput();
        HandleDrawWeaponInput();
        HandleSwitchWeaponInput();
    }

    private void HandleInteractionInput()
    {
        if (interactInput)
        {
            interactInput = false;
            interactionManager.HandleInteractionInput();
        }
    }

    private void HandleMovementInput() 
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }
    
    private void HandleSprintingInput() 
    {
        if (sprintInput && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true; 
        }
        else 
        {
            playerLocomotion.isSprinting = false; 
        }
    }

    private void HandleJumpingInput() 
    {
        if (jumpInput)
        {
            jumpInput = false;
        }
    }

    private void HandleRollInput() 
    {
        if (rollInput)
        {
            rollInput = false;
            playerLocomotion.HandleRoll();
        }
    }

    private void HandleBlockInput()
    {
        if (blockInput)
        {
            playerCombat.isBlocking = true;
        }
        else
        {
            playerCombat.isBlocking = false; 
        }
    }

    private void HandleAttackInput()
    {
        if (attackInput)
        {
            attackHoldTime += Time.deltaTime;

            if (attackHoldTime >= heavyAttackThreshold && !attackHeld)
            {
                attackHeld = true;
                playerCombat.HandleHeavyAttack();
            }
        }
        else if (!attackInput && attackHoldTime > 0)
        {
            if (!attackHeld && attackHoldTime < heavyAttackThreshold)
            {
                playerLocomotion.StopMovement();
                playerCombat.HandleLightAttack();
            }

            attackHoldTime = 0f;
            attackHeld = false;
        }
    }

    private void HandleDrawWeaponInput()
    {
        if (drawWeaponInput)
        {
            drawWeaponInput = false;
            playerCombat.ToggleWeaponDrawn();
        }
    }

    private void HandleSwitchWeaponInput()
    {
        if (switchWeaponInput)
        {
            switchWeaponInput = false;
            CycleWeapon();
        }
    }

    private void CycleWeapon()
    {
        // Cycle through available weapons for testing 
        WeaponType newWeaponType;
        
        switch (playerCombat.currentEquippedWeapon)
        {
            case WeaponType.Unarmed:
                newWeaponType = WeaponType.GreatSword;
                break;
            case WeaponType.GreatSword:
                newWeaponType = WeaponType.SwordAndShield;
                break;
            case WeaponType.SwordAndShield:
                newWeaponType = WeaponType.Unarmed;
                break;
            default:
                newWeaponType = WeaponType.Unarmed;
                break;
        }
        
        playerCombat.SwitchWeapon(newWeaponType);
    }
}