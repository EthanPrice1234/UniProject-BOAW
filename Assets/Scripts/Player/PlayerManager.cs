using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    PlayerCombat playerCombat;
    CameraManager cameraManager;
    Animator animator; 

    public bool isInteracting;
    public bool isGrounded;
    public bool isRolling;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();     
        playerCombat = GetComponent<PlayerCombat>();   
        cameraManager = FindAnyObjectByType<CameraManager>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate() 
    {
        playerLocomotion.HandleAllMovement();
        playerCombat.HandleAllCombat();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        isInteracting = animator.GetBool("isInteracting");
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRolling", isRolling);
    }
}
