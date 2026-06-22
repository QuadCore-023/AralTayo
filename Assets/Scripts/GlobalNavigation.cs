using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalNavigation : MonoBehaviour
{
    [Header("Navigation Buttons")]
    public Button navBedroomButton;
    public Button navDiningButton;
    public Button navHallwayButton;
    public Button navTabletButton;

    [Header("Navigation Mode")]
    [Tooltip("If true, clicking a button loads a new scene. If false, it toggles UI Panels in the same scene.")]
    public bool useSceneNavigation = true;

    [Header("Scene Names (Scene Mode)")]
    public string bedroomSceneName = "BedroomScene";
    public string diningSceneName = "DiningScene";
    public string hallwaySceneName = "HallwayScene";
    public string tabletSceneName = "TabletScene";

    [Header("UI Panels (Panel Mode)")]
    public GameObject bedroomPanel;
    public GameObject diningPanel;
    public GameObject hallwayPanel;
    public GameObject tabletPanel;

    void Start() {
        if (navBedroomButton != null) {
            navBedroomButton.onClick.AddListener(OnBedroomClicked);
        }
        if (navDiningButton != null) {
            navDiningButton.onClick.AddListener(OnDiningClicked);
        }
        if (navHallwayButton != null) {
            navHallwayButton.onClick.AddListener(OnHallwayClicked);
        }
        if (navTabletButton != null) {
            navTabletButton.onClick.AddListener(OnTabletClicked);
        }
    }

    private void OnBedroomClicked() {
        NavigateTo(bedroomSceneName, bedroomPanel);
    }

    private void OnDiningClicked() {
        NavigateTo(diningSceneName, diningPanel);
    }

    private void OnHallwayClicked() {
        NavigateTo(hallwaySceneName, hallwayPanel);
    }

    private void OnTabletClicked() {
        NavigateTo(tabletSceneName, tabletPanel);
    }

    private void NavigateTo(string sceneName, GameObject panel) {
        if (useSceneNavigation) {
            if (!string.IsNullOrEmpty(sceneName)) {
                SceneManager.LoadScene(sceneName);
            } else {
                Debug.LogWarning("Target scene name is empty!");
            }
        } else {
            // Toggles active panels in the scene
            if (bedroomPanel != null) bedroomPanel.SetActive(bedroomPanel == panel);
            if (diningPanel != null) diningPanel.SetActive(diningPanel == panel);
            if (hallwayPanel != null) hallwayPanel.SetActive(hallwayPanel == panel);
            if (tabletPanel != null) tabletPanel.SetActive(tabletPanel == panel);
        }
    }
}
