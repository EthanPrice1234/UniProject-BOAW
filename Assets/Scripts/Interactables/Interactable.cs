using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    public float radius = 0.6f;
    public string interactbleText;
    public bool isInteractable = true;
    public GameObject interactionPrompt;
    public TextMeshProUGUI interactableTextBox;

    protected virtual void Awake()
    {
        // Find and assign the interaction prompt
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true); 
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Interact Text")
            {
                interactionPrompt = obj;
                break;
            }
        }

        // Find and assign the interaction text
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI text in allTexts)
        {
            if (text.CompareTag("InteractText"))
            {
                interactableTextBox = text;
                break;
            }
        }

        if (interactionPrompt == null)
        {
            Debug.LogWarning("Couldn't find Interact Text");
        }
        if (interactableTextBox == null)
        {
            Debug.LogWarning("Couldn't find Interact Text Box");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);     
    }

    public virtual void Interact(PlayerManager playerManager)
    {
        // Base interact method
    }

    public virtual void ShowInteractionPrompt(bool show)
    {
        if (interactionPrompt != null && isInteractable)
        {
            // Set the interact prompt
            interactableTextBox.text = interactbleText;
            interactionPrompt.SetActive(show);
        }
    }
}
