using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensures the application always starts in the configured startup scene and displays the start screen UI panel.
/// Attach this component to a GameObject present in every possible entry scene (e.g., a bootstrap object).
/// </summary>
public class StartupManager : MonoBehaviour
{
    [SerializeField] private string startupSceneName = "Startup"; // set in Inspector
    [SerializeField] private GameObject startScreenPanel; // assign the start screen UI panel

    private void Awake()
    {
        // If the current scene is not the startup scene, load it.
        if (SceneManager.GetActiveScene().name != startupSceneName)
        {
            SceneManager.LoadScene(startupSceneName);
        }
    }

    private void Start()
    {
        // Activate the start screen UI panel after the scene loads.
        if (startScreenPanel != null)
        {
            startScreenPanel.SetActive(true);
        }
    }
}
