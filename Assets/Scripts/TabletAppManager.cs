using UnityEngine;
using UnityEngine.SceneManagement;

public class TabletAppManager : MonoBehaviour
{
    [Header("Global UI Elements")]
    [Tooltip("Drag your persistent Sidebar or NavBar GameObject here to hide it inside apps")]
    public GameObject sideBar;

    [Header("Tablet Core Layout")]
    [Tooltip("Drag the AppDrawerContainer GameObject here")]
    public GameObject appDrawerContainer;

    [Tooltip("Drag the BackButton GameObject here")]
    public GameObject backButton;

    [Header("Tablet Main App Panels")]
    [Tooltip("Drag ControlPanelScreen here")]
    public GameObject controlPanelScreen;

    [Tooltip("Drag CustomizeScreen here")]
    public GameObject customizeScreen;

    [Header("Settings & Shop Panels")]
    [Tooltip("Drag SettingsScreen here")]
    public GameObject settingsScreen;

    [Tooltip("Drag ShopScreen here")]
    public GameObject shopScreen;

    [Header("Control Panel Sub-Navigation")]
    [Tooltip("Drag the ControlPanelButtons container here")]
    public GameObject controlPanelButtonsContainer;

    [Tooltip("Drag ControlPanelLogsScreen here")]
    public GameObject controlPanelLogsScreen;

    [Tooltip("Drag ControlPanelImportScreen here")]
    public GameObject controlPanelImportScreen;

    [Tooltip("Drag ControlPanelExportScreen here")]
    public GameObject controlPanelExportScreen;

    private void OnEnable()
    {
        // Whenever the tablet is opened via the NavBar, reset it to the app grid home layout and ensure navbar is visible
        CloseAllApps();
    }

    /// <summary>
    /// Opens a specific main app panel, hides the home grid and global navigation bar, and displays the back button.
    /// </summary>
    public void OpenApp(GameObject appPanelToShow)
    {
        // 1. Hide the app selection drawer
        if (appDrawerContainer != null) appDrawerContainer.SetActive(false);

        // 2. Hide the persistent sidebar/navbar to give the app full screen priority
        if (sideBar != null) sideBar.SetActive(false);

        // 3. Disable all individual app screens and sub-screens to prevent overlaps
        HideAllMainAppPanels();
        HideAllControlPanelSubScreens();

        // 4. Show the chosen app screen
        if (appPanelToShow != null)
        {
            appPanelToShow.SetActive(true);
        }

        // 5. Special Case: Handling Control Panel layout
        if (appPanelToShow == controlPanelScreen)
        {
            // Ensure its navigation container button grid is turned on
            if (controlPanelButtonsContainer != null)
            {
                controlPanelButtonsContainer.SetActive(true);
            }

            // FIX: Instantly load the Logs Sub-Screen as the landing view so it never opens blank
            if (controlPanelLogsScreen != null)
            {
                controlPanelLogsScreen.SetActive(true);
            }
        }

        // 6. Reveal the back button so the user can escape the app
        if (backButton != null)
        {
            backButton.SetActive(true);
        }
    }

    /// <summary>
    /// Triggered by the BackButton. Hides active apps, restores the app drawer grid, restores the sidebar, and hides itself.
    /// </summary>
    public void CloseAllApps()
    {
        // 1. Hide all internal main views and sub-screens
        HideAllMainAppPanels();
        HideAllControlPanelSubScreens();

        // 2. Hide the control panel sub-buttons container explicitly
        if (controlPanelButtonsContainer != null) controlPanelButtonsContainer.SetActive(false);

        // 3. Re-enable the home App Drawer container
        if (appDrawerContainer != null) appDrawerContainer.SetActive(true);

        // 4. Bring back the persistent sidebar/navbar for room navigation
        if (sideBar != null) sideBar.SetActive(true);

        // 5. Hide the back button since we are back on the main menu
        if (backButton != null)
        {
            backButton.SetActive(false);
        }
    }

    /// <summary>
    /// Internal helper method to systematically disable all main app views.
    /// </summary>
    private void HideAllMainAppPanels()
    {
        if (controlPanelScreen != null) controlPanelScreen.SetActive(false);
        if (customizeScreen != null) customizeScreen.SetActive(false);
        if (settingsScreen != null) settingsScreen.SetActive(false);
        if (shopScreen != null) shopScreen.SetActive(false);
    }

    /// <summary>
    /// Internal helper method to systematically disable all Control Panel sub-screens.
    /// </summary>
    private void HideAllControlPanelSubScreens()
    {
        if (controlPanelLogsScreen != null) controlPanelLogsScreen.SetActive(false);
        if (controlPanelImportScreen != null) controlPanelImportScreen.SetActive(false);
        if (controlPanelExportScreen != null) controlPanelExportScreen.SetActive(false);
    }

    /// <summary>
    /// Switches between the sub-screens inside the Control Panel while keeping the sub-navigation persistent.
    /// </summary>
    public void OpenControlPanelSubScreen(GameObject subScreenToShow)
    {
        // Hide all sub-screens first to prevent overlapping content layers
        HideAllControlPanelSubScreens();

        // Ensure the sub-navigation container itself STAYS active and visible
        if (controlPanelButtonsContainer != null)
        {
            controlPanelButtonsContainer.SetActive(true);
        }

        // Show the selected sub-screen
        if (subScreenToShow != null)
        {
            subScreenToShow.SetActive(true);
        }
    }

    // --------------------------------------------------------
    // Public UI Button Wrapper Functions for Main Apps
    // --------------------------------------------------------
    public void OpenControlPanel() => OpenApp(controlPanelScreen);
    public void OpenCustomize() => OpenApp(customizeScreen);
    public void OpenSettings() => OpenApp(settingsScreen);
    public void OpenShop() => OpenApp(shopScreen);

    // --------------------------------------------------------
    // Public UI Button Wrapper Functions for Control Panel Sub-Screens
    // --------------------------------------------------------
    public void OpenControlPanelLogs() => OpenControlPanelSubScreen(controlPanelLogsScreen);
    public void OpenControlPanelImport() => OpenControlPanelSubScreen(controlPanelImportScreen);
    public void OpenControlPanelExport() => OpenControlPanelSubScreen(controlPanelExportScreen);

    /// <summary>
    /// Closes this scene completely and opens the startup menu scene.
    /// </summary>
    public void GoToMainMenuScene()
    {
        SceneManager.LoadScene("startup");
    }
}