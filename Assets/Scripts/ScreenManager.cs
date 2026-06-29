using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [Header("Main Environment Screens")]
    [Tooltip("Drag the BedroomScreen GameObject here")]
    public GameObject bedroomScreen;

    [Tooltip("Drag the DiningRoomScreen GameObject here")]
    public GameObject diningRoomScreen;

    [Tooltip("Drag the HallwayScreen GameObject here")]
    public GameObject hallwayScreen;

    [Tooltip("Drag the TabletScreen GameObject here")]
    public GameObject tabletScreen;

    [Header("Initialization")]
    [Tooltip("The screen that should be active when the app starts")]
    public GameObject startingScreen;

    private void Start()
    {
        // On app launch, ensure only the starting screen is visible.
        if (startingScreen != null)
        {
            ShowScreen(startingScreen);
        }
        else if (bedroomScreen != null)
        {
            // Fallback to bedroom if nothing is assigned
            ShowScreen(bedroomScreen);
        }
    }

    /// <summary>
    /// Core method to handle screen swapping. 
    /// Turns off all main screens, then turns on the target screen.
    /// </summary>
    public void ShowScreen(GameObject screenToShow)
    {
        // 1. Disable all screens first to prevent overlap
        if (bedroomScreen != null) bedroomScreen.SetActive(false);
        if (diningRoomScreen != null) diningRoomScreen.SetActive(false);
        if (hallwayScreen != null) hallwayScreen.SetActive(false);
        if (tabletScreen != null) tabletScreen.SetActive(false);

        // 2. Enable the requested screen
        if (screenToShow != null)
        {
            screenToShow.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ScreenManager: The screen you are trying to show is missing or unassigned!");
        }
    }

    // --------------------------------------------------------
    // Helper wrappers for Unity UI Button OnClick() Events
    // --------------------------------------------------------
    public void GoToBedroom() => ShowScreen(bedroomScreen);
    public void GoToDiningRoom() => ShowScreen(diningRoomScreen);
    public void GoToHallway() => ShowScreen(hallwayScreen);
    public void GoToTablet() => ShowScreen(tabletScreen);
}