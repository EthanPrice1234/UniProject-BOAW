using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float checkRadius = 1.0f;
    public LayerMask interactableLayers;

    private Interactable currentInteractable;
    private bool isNearInteractable = false;
    private PlayerManager playerManager;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    void Update()
    {
        CheckForInteractables();
    }

    void CheckForInteractables()
    {
        // Check for interactables in the radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, interactableLayers);

        bool foundInteractable = false;
        Interactable nearestInteractable = null;
        float closestDistance = float.MaxValue;

        // Get nearest interactable
        foreach (Collider col in colliders)
        {
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null)
            {
                foundInteractable = true;
                
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance; 
                    nearestInteractable = interactable;
                }
            }
        }

        // Found an interactable and it's the nearest one
        if (foundInteractable && nearestInteractable != null) 
        {
            if (currentInteractable != nearestInteractable)
            {
                if (currentInteractable != null)
                    currentInteractable.ShowInteractionPrompt(false);

                currentInteractable = nearestInteractable;
            }

            currentInteractable.ShowInteractionPrompt(true);
            isNearInteractable = true;
        }
        else 
        {
            // No interactable found or it's not the nearest one
            if (currentInteractable != null)
            {
                currentInteractable.ShowInteractionPrompt(false);
                currentInteractable = null;
            }

            isNearInteractable = false;
        }
    }

    public void HandleInteractionInput() 
    {
        if (isNearInteractable && currentInteractable != null)
        {
            currentInteractable.Interact(playerManager);
        }
    }
}
