using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class MainMenu : MonoBehaviour
{
    [SerializeField] UIDocument mainMenuDocument;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private UnityEngine.UI.Image fadePanel;

    private Button playButton;
    private bool isTransitioning = false;

    private void Awake()
    {
        VisualElement root = mainMenuDocument.rootVisualElement;
        playButton = root.Q<Button>("PlayButton");

        playButton.clickable.clicked += PlayGame;

        // Find fade panel if not assigned
        if (fadePanel == null)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("FadePanel"))
                {
                    fadePanel = obj.GetComponent<UnityEngine.UI.Image>();
                    break;
                }
            }
        }

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = new Color(0, 0, 0, 0);
        }
    }   

    private void PlayGame() 
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToGame());
        }
    }

    private IEnumerator TransitionToGame()
    {
        isTransitioning = true;

        // Fade to black
        if (fadePanel != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                fadePanel.color = new Color(0, 0, 0, newAlpha);
                yield return null;
            }
            fadePanel.color = new Color(0, 0, 0, 1f);
        }

        // Randomly select a stage 
        int randomStage = Random.Range(0, 2);
        if (randomStage == 0)
        {
            SceneManager.LoadScene("PlainsStage");
        }
        else if (randomStage == 1)
        {
            SceneManager.LoadScene("SnowyStage");
        }
    }
}
