using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP;

public class StatItemPickup : Interactable
{
    public StatItem item;
    PlayerInventory playerInventory;
    PlayerLocomotion playerLocomotion;
    Rigidbody playerRigidBody;
    AnimatorManager animatorManager;
    PlayerStats playerStats;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        PickUpItem(playerManager);
    }

    private void PickUpItem(PlayerManager playerManager)
    {

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
        playerRigidBody = playerLocomotion.GetComponent<Rigidbody>();
        animatorManager = playerManager.GetComponentInChildren<AnimatorManager>();
        playerStats = playerManager.GetComponent<PlayerStats>();

        // Stop movement and play pickup animation
        playerRigidBody.velocity = Vector3.zero;
        animatorManager.PlayTargetAnimation("Pickup Item", true);
        playerInventory.itemsInventory.Add(item);
        
        playerStats.ApplyStatItem(item.itemName);
        
        isInteractable = false;
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        ShowInteractionPrompt(false);
        Destroy(gameObject);
    }
}
