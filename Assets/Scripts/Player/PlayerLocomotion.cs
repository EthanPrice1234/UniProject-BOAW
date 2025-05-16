using System.Collections;
using System.Collections.Generic;
// using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    PlayerStats playerStats;
    AnimatorManager animatorManager; 
    InputManager inputManager;

    public Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;

    [Header("Ground and Air Detection Stats")]
    [SerializeField] 
    float groundDetectionRayStartPosition = 0.5f; 
    [SerializeField] 
    float groundDetectionRayDistance = 0.6f; 
    [SerializeField]
    LayerMask groundLayer;
    public float inAirTimer;

    [Header("Falling")]
    public float fallingVelocity = 80f; 

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isJumping; 

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float movementSpeed = 5;
    public float sprintingSpeed = 7;
    public float rotationSpeed = 15;
    public float fallingSpeed = 45;

    [Header("Jump Speeds")]
    public float fallSpeed = 15f;

    [Header("Roll Presets")]
    public bool isRolling;
    public float rollSpeed = 8f; 
    public float rollDuration = 0.6f; 
    public float rollTimer; 
    private Vector3 rollDirection;
    
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>(); 
        playerStats = GetComponent<PlayerStats>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;

        playerRigidbody.useGravity = true;
        playerManager.isGrounded = true;
    }

    public void WakeUp() 
    {
        animatorManager.PlayTargetAnimation("Getting Up", true); 
    }

    public void HandleAllMovement() 
    {   
        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            
            Vector3 rollVelocity = rollDirection * (rollSpeed * playerStats.speedMultiplier);
            playerRigidbody.velocity = new Vector3(rollVelocity.x, playerRigidbody.velocity.y, rollVelocity.z);
            
            if (rollTimer >= rollDuration)
            {
                isRolling = false;
                playerManager.isRolling = false;
            }
        }

        if (playerManager.isInteracting)
        {
            return;
        }

        // HandleFalling(); // removed falling and jumping, want to focus on combat on the ground
        HandleMovement();
        HandleRotation();
    }

    private void HandleFalling()
    {
        Vector3 rayStart = transform.position + Vector3.up * groundDetectionRayStartPosition;
        RaycastHit hit;
        
        Debug.DrawRay(rayStart, Vector3.down * groundDetectionRayDistance, Color.red);
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, groundDetectionRayDistance, groundLayer))
        {
            //isGrounded = true;
            playerManager.isGrounded = true;
            animatorManager.animator.SetBool("isGrounded", true);
            inAirTimer = 0;
            Debug.Log("Player is grounded!");
        }
        else
        {
            // isGrounded = false;
            playerManager.isGrounded = false;
            animatorManager.animator.SetBool("isGrounded", false);
            inAirTimer += Time.deltaTime;
            
            Vector3 currentVelocity = playerRigidbody.velocity;
            currentVelocity.y = -fallingVelocity;
            playerRigidbody.velocity = currentVelocity;
        }
    }

    private void HandleMovement() 
    {
        // If in air only move horizontally
        if (!playerManager.isGrounded)
        {
            moveDirection = cameraObject.forward * inputManager.verticalInput;
            moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (isSprinting)
            {
                moveDirection = moveDirection * (sprintingSpeed * playerStats.speedMultiplier); 
            }
            else 
            {
                if (inputManager.moveAmount >= 0.5f){
                    moveDirection = moveDirection * (movementSpeed * playerStats.speedMultiplier); 
                }
                else
                {
                    moveDirection = moveDirection * (walkingSpeed * playerStats.speedMultiplier); 
                }
            }
        
            Vector3 airVelocity = moveDirection;
            airVelocity.y = playerRigidbody.velocity.y; 
            playerRigidbody.velocity = airVelocity;
            return;
        }

        // If rolling only use rolling movement
        if (isRolling)
        {
            return;
        }

        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // Move faster when sprinting
        if (isSprinting)
        {
            moveDirection = moveDirection * (sprintingSpeed * playerStats.speedMultiplier); 
        }
        else 
        {
            if (inputManager.moveAmount >= 0.5f){
                moveDirection = moveDirection * (movementSpeed * playerStats.speedMultiplier); 
            }
            else
            {
                moveDirection = moveDirection * (walkingSpeed * playerStats.speedMultiplier); 
            }
        }
    
        Vector3 groundVelocity = moveDirection;
        groundVelocity.y = playerRigidbody.velocity.y;
        playerRigidbody.velocity = groundVelocity;
    }

    private void HandleRotation() 
    {
        Vector3 targetDirection = Vector3.zero;

        // Rotate player based on camera direction
        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize(); 
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero) 
                targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRoation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRoation;
    }
    
    public void ApplyJumpForce()
    {
        // Jumping removed for now, want to focus on ground combat. 
        // isGrounded = false;
        playerManager.isGrounded = false;
        animatorManager.animator.SetBool("isGrounded", false);
    }

    public void HandleRoll()
    {
        if (playerManager.isGrounded && !isRolling && !playerManager.isInteracting)
        {
            isRolling = true;
            rollTimer = 0;
            
            rollDirection = moveDirection.normalized;

            if (rollDirection.magnitude < 0.1f)
                rollDirection = transform.forward;
                            
            playerManager.isRolling = true;
            animatorManager.PlayTargetAnimation("Roll", false);
        }
    }

    public void StopMovement() 
    {
        Vector3 currentVelocity = playerRigidbody.velocity;
        playerRigidbody.velocity = new Vector3(0, currentVelocity.y, 0);
    }

    private void OnDrawGizmos()
    {
        // Draw the ground detection ray
        Gizmos.color = Color.yellow;
        Vector3 rayStart = transform.position + Vector3.up * groundDetectionRayStartPosition;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * groundDetectionRayDistance);
    }
}