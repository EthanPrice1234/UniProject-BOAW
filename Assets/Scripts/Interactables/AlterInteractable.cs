using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlterInteractable : Interactable
{
    [Header("Timer Settings")]
    public float eventDuration = 90f;
    public GameObject timerPanel;
    public TextMeshProUGUI timerText;

    private bool isEventActive = false;
    private float currentTime;
    private PlayerManager playerManager;

    // Directors
    DirectorStageManager directorStageManager;
    DirectorEnemyManager directorEnemyManager;
    DirectorBindingManager directorBindingManager;

    protected override void Awake()
    {
        base.Awake(); 

        directorStageManager = GameObject.FindGameObjectWithTag("Director").GetComponent<DirectorStageManager>();
        directorEnemyManager = GameObject.FindGameObjectWithTag("Director").GetComponent<DirectorEnemyManager>();
        directorBindingManager = GameObject.FindGameObjectWithTag("Director").GetComponent<DirectorBindingManager>();
        // Find and assign the timer panel
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("LockdownTimer"))
            {
                timerPanel = obj;
                timerText = timerPanel.GetComponentInChildren<TextMeshProUGUI>(true);
                break;
            }
        }

        if (timerPanel == null)
        {
            Debug.LogWarning($"Could not find LockdownTimer");
        }
    }

    private void Start()
    {
        if (timerPanel != null)
            timerPanel.SetActive(false);
    }

    public override void Interact(PlayerManager playerManager)
    {
        if (isEventActive)
            return;

        this.playerManager = playerManager;

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
        StartEvent();
    }

    private void StartEvent()
    {
        isEventActive = true;
        currentTime = eventDuration;
        
        if (timerPanel != null)
            timerPanel.SetActive(true);

        StartCoroutine(EventTimerRoutine());
        directorEnemyManager.alterEvent = true;
        
        AnimatorManager animatorManager = playerManager.GetComponentInChildren<AnimatorManager>();
        animatorManager.PlayTargetAnimation("Interact", true);
    }

    private IEnumerator EventTimerRoutine()
    {
        while (currentTime > 0)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60);
                int seconds = Mathf.FloorToInt(currentTime % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            currentTime -= Time.deltaTime;
            yield return null;
        }

        EndEvent();
    }

    private void EndEvent()
    {
        directorBindingManager.trialsCompleted++;
        directorBindingManager.HandleBinding();

        isEventActive = false;
        directorEnemyManager.alterEvent = false;
        
        if (timerPanel != null)
            timerPanel.SetActive(false);

        directorStageManager.NextStage();
    }
}