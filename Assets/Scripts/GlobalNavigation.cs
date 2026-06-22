using UnityEngine;
using UnityEngine.UI;

public class GlobalNavigation : MonoBehaviour
{
    [Header("Navigation Buttons")]
    public Button navBedroomButton;
    public Button navDiningButton;
    public Button navHallwayButton;
    public Button navTabletButton;

    [Header("UI Panels")]
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
        ShowPanel(bedroomPanel);
    }

    private void OnDiningClicked() {
        ShowPanel(diningPanel);
    }

    private void OnHallwayClicked() {
        ShowPanel(hallwayPanel);
    }

    private void OnTabletClicked() {
        ShowPanel(tabletPanel);
    }

    private void ShowPanel(GameObject panel) {
        if (bedroomPanel != null) bedroomPanel.SetActive(bedroomPanel == panel);
        if (diningPanel != null) diningPanel.SetActive(diningPanel == panel);
        if (hallwayPanel != null) hallwayPanel.SetActive(hallwayPanel == panel);
        if (tabletPanel != null) tabletPanel.SetActive(tabletPanel == panel);
    }
}
