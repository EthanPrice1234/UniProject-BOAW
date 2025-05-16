using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP;
using TMPro;

public class ChestInteractable : Interactable
{
    public Animator animator;
    private PlayerManager playerManager;
    private PlayerStats playerStats;
    public bool isOpen = false;
    public bool canInteract = true;
    public int chestCost = 10;

    [Header("Item Spawn Settings")]
    public GameObject[] itemPrefabs; 
    public Transform itemSpawnPoint;
    public float spawnDistance = 1.5f; 
    public float spawnHeight = 0.5f; 

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerStats = playerManager.GetComponent<PlayerStats>();

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // If no item spawn point, create default one for testing
        if (itemSpawnPoint == null)
        {
            GameObject spawnPoint = new GameObject("ItemSpawnPoint");
            spawnPoint.transform.SetParent(transform);
            spawnPoint.transform.localPosition = new Vector3(0, spawnHeight, spawnDistance);
            itemSpawnPoint = spawnPoint.transform;
        }
    }

    public override void Interact(PlayerManager playerManager)
    {
        AnimatorManager animatorManager;

        if (!canInteract)
            return;

        // If chest is not open and player has enough gold, open the chest
        if (!isOpen && playerStats.gold >= chestCost)
        {
            playerStats.RemoveGold(chestCost);
            animatorManager = playerManager.GetComponentInChildren<AnimatorManager>();
            animatorManager.PlayTargetAnimation("Interact", true);
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (!isOpen && canInteract)
        {
            isOpen = true;
            canInteract = false;
            isInteractable = false;
            animator.Play("Open Chest");
            
            if (GetComponent<Collider>() != null)
            {
                GetComponent<Collider>().enabled = false;
            }
            
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            ShowInteractionPrompt(false);
            
            SpawnRandomItem();
        }
    }

    private void SpawnRandomItem()
    {
        if (itemPrefabs.Length == 0)
        {
            Debug.LogWarning("No item prefabs set.");
            return;
        }

        // Random item from the prefabs array
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject selectedItem = itemPrefabs[randomIndex];

        if (selectedItem != null)
        {
            // Spawn the item at the spawn point
            GameObject spawnedItem = Instantiate(selectedItem, itemSpawnPoint.position, Quaternion.identity);

            // Random rotation
            spawnedItem.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            
            // Random offset
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                0,
                Random.Range(-0.3f, 0.3f)
            );
            spawnedItem.transform.position += randomOffset;
        }
    }
} 