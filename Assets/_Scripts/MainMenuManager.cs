 using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Fusion;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    
    private string playerName = "";
    
    private void Start()
    {
        // Setup button listener
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        
        // Setup input field
        if (playerNameInput != null)
        {
            playerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
            playerNameInput.text = "Player" + Random.Range(1, 1000);
        }
        
        // Hide loading panel initially
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
    
    private void OnPlayerNameChanged(string name)
    {
        playerName = name;
    }
    
    private void StartGame()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player" + Random.Range(1, 1000);
        }
        
        StartCoroutine(LoadGameScene());
    }
    
    private IEnumerator LoadGameScene()
    {
        // Store player name
        PlayerPrefs.SetString("PlayerName", playerName);
        
        // Show loading panel
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        
        // Simulate loading progress
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * 0.5f;
            
            if (loadingBar != null)
                loadingBar.value = progress;
            
            if (loadingText != null)
                loadingText.text = $"Loading... {(progress * 100):F0}%";
            
            yield return null;
        }
        
        // Wait for Fusion to be ready before loading scene
        Debug.Log("MainMenu: Waiting for Fusion to be ready...");
        yield return new WaitForSeconds(2f); // Wait for Fusion connection
        
        // Load game scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false; // Don't activate scene immediately
        
        while (!asyncLoad.isDone)
        {
            float sceneProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9f is the max progress before activation
            if (loadingBar != null)
                loadingBar.value = sceneProgress;
            
            if (loadingText != null)
                loadingText.text = $"Loading Game Scene... {(sceneProgress * 100):F0}%";
            
            // When loading is complete (90%), wait a bit then activate
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f); // Wait for Fusion to be stable
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        // Wait for Fusion to connect and spawn players
        yield return new WaitForSeconds(2f);
        
        // In Shared Mode, Fusion Bootstrap handles the connection automatically
        Debug.Log("MainMenu: Scene loaded, Fusion Bootstrap will handle connection");
    }
}
