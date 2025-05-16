using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class DirectorStageManager : MonoBehaviour
{
    private GameObject player;
    private GameObject alter;

    private Scene currentScene;
    public string sceneName;
    
    [Header("Chest Settings")]
    public GameObject chestPrefab;
    public int minChests = 6;
    public int maxChests = 9;

    [Header("Transition Settings")]
    public float fadeDuration = 1.5f;
    public Image fadePanel;
    public bool isTransitioning = false;

    protected void Start()
    {
        StageSetup();
    }

    private void StageSetup()
    {
        Debug.Log("Loading Stage");

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;

        CombatController.Instance.DisengageAll();

        // Find and setup fade panel
        if (fadePanel == null)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("FadePanel"))
                {
                    fadePanel = obj.GetComponent<Image>();
                    break;
                }
            }
        }

        // Start with black screen
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = new Color(0, 0, 0, 1);
        }

        StartCoroutine(InitializeStage());
    }

    private IEnumerator InitializeStage()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        alter = GameObject.FindGameObjectWithTag("Alter");

        if (player == null || alter == null)
        {
            Debug.LogError("Player or Alter not found. Make sure they have the correct tags.");
            yield break;
        }

        // Select a random spawn point pair (1-3)
        int spawnPointIndex = Random.Range(0, 3); 

        // Find the corresponding spawn points
        GameObject[] alterSpawnPoints = GameObject.FindGameObjectsWithTag("AlterSpawnPoint");
        GameObject[] playerSpawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");

        // Sort the spawn points by name for correct pairing
        System.Array.Sort(alterSpawnPoints, (a, b) => a.name.CompareTo(b.name));
        System.Array.Sort(playerSpawnPoints, (a, b) => a.name.CompareTo(b.name));

        if (alterSpawnPoints.Length < 3 || playerSpawnPoints.Length < 3)
        {
            Debug.LogError("Not enough spawn points found. Make sure there are 3 of each type.");
            yield break;
        }

        // Position the alter at its spawn point
        alter.transform.position = alterSpawnPoints[spawnPointIndex].transform.position;
        alter.transform.rotation = alterSpawnPoints[spawnPointIndex].transform.rotation;

        // Disable, move, and re-enable the player
        player.SetActive(false);
        player.transform.position = playerSpawnPoints[spawnPointIndex].transform.position;
        player.transform.rotation = playerSpawnPoints[spawnPointIndex].transform.rotation;
        player.SetActive(true);

        Debug.Log($"Selected spawn point pair {spawnPointIndex + 1}");
        Debug.Log("Player position: " + player.transform.position);
        Debug.Log("Alter position: " + alter.transform.position);

        // Spawn chests
        SpawnChests();

        // Wait a frame so everything is set up
        yield return null;

        // Fade in
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeRoutine(0f));
        }
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        if (fadePanel == null) yield break;

        float startAlpha = fadePanel.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // Fade in or out
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, targetAlpha);
    }

    public void NextStage()
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToNextStage());
        }
    }

    private IEnumerator TransitionToNextStage()
    {
        isTransitioning = true;

        // Fade to black
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeRoutine(1f));
        }

        // Load the next scene randomly
        int randomStage = Random.Range(0, 2);
        if (randomStage == 0)
        {
            SceneManager.LoadScene("PlainsStage");
        }
        else if (randomStage == 1)
        {
            SceneManager.LoadScene("SnowyStage");
        }

        Debug.Log("Not sure we even reach this code?");
        
        // Wait for the scene to be fully loaded
        yield return new WaitForSeconds(0.1f);
        StageSetup();
        
        // Re-find the fade panel in the new scene
        if (fadePanel == null)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("FadePanel"))
                {
                    fadePanel = obj.GetComponent<Image>();
                    break;
                }
            }
        }

        if (fadePanel != null)
        {
            // Ensure the panel is visible and black
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = new Color(0, 0, 0, 1);
            
            // Fade back to transparent
            yield return StartCoroutine(FadeRoutine(0f));
        }

        isTransitioning = false;
    }

    private void SpawnChests()
    {
        if (chestPrefab == null)
        {
            Debug.LogError("Chest prefab not assigned in DirectorStageManager.");
            return;
        }

        // Get all chest spawn points
        GameObject[] chestSpawnPoints = GameObject.FindGameObjectsWithTag("ChestSpawnPoint");
        
        if (chestSpawnPoints.Length == 0)
        {
            Debug.LogError("No chest spawn points found. Make sure they have the ChestSpawnPoint' tag.");
            return;
        }

        List<GameObject> availableSpawnPoints = new List<GameObject>(chestSpawnPoints);
        
        // Random number of chests to spawn
        int numChests = Random.Range(minChests, maxChests + 1);
        numChests = Mathf.Min(numChests, availableSpawnPoints.Count); // check no more chests than points

        for (int i = 0; i < numChests; i++)
        {
            // Get random spawn point
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            GameObject spawnPoint = availableSpawnPoints[randomIndex];
            
            // Spawn chest
            GameObject chest = Instantiate(chestPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            
            // Remove used spawn point
            availableSpawnPoints.RemoveAt(randomIndex);
        }

        Debug.Log($"Spawned {numChests} chests");
    }
}
