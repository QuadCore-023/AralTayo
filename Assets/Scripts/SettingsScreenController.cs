using UnityEngine;

public class SettingsScreenController : MonoBehaviour
{
    [Header("Sub-Panel GameObjects")]
    [Tooltip("Drag the internal SettingsSettingsScreen here")]
    public GameObject settingsPanel;
    [Tooltip("Drag the internal SettingsAboutScreen / TabletAboutPanelScreen here")]
    public GameObject aboutPanel;
    [Tooltip("Drag the internal SettingsHelpScreen / TabletHelpPanelScreen here")]
    public GameObject helpPanel;
    [Tooltip("Drag the internal SettingsTermsScreen / TabletTermsPanelScreen here")]
    public GameObject termsPanel;

    private void OnEnable()
    {
        // Resets the view back to the main settings page automatically whenever the Settings app opens
        SwitchToPanel(settingsPanel);
    }

    /// <summary>
    /// Activates the selected sub-panel target and shuts down all other overlapping sub-panels.
    /// </summary>
    public void SwitchToPanel(GameObject targetPanel)
    {
        if (settingsPanel != null) settingsPanel.SetActive(settingsPanel == targetPanel);
        if (aboutPanel != null) aboutPanel.SetActive(aboutPanel == targetPanel);
        if (helpPanel != null) helpPanel.SetActive(helpPanel == targetPanel);
        if (termsPanel != null) termsPanel.SetActive(termsPanel == targetPanel);
    }

    // ------------------------------------------------------------------------
    // PUBLIC METHODS EXPOSED TO UNITY'S ONCLICK() INSPECTOR
    // ------------------------------------------------------------------------

    /// <summary>
    /// Drag this function into the 'settingsbutton' OnClick event.
    /// </summary>
    public void OpenSettingsPanel()
    {
        SwitchToPanel(settingsPanel);
    }

    /// <summary>
    /// Drag this function into the 'aboutbutton' OnClick event.
    /// </summary>
    public void OpenAboutPanel()
    {
        SwitchToPanel(aboutPanel);
    }

    /// <summary>
    /// Drag this function into the 'helpbutton' OnClick event.
    /// </summary>
    public void OpenHelpPanel()
    {
        SwitchToPanel(helpPanel);
    }

    /// <summary>
    /// Drag this function into the 'termsbutton' OnClick event.
    /// </summary>
    public void OpenTermsPanel()
    {
        SwitchToPanel(termsPanel);
    }
}