using UnityEngine;
using UnityEngine.UI;

public class CompanionStatsManager : MonoBehaviour
{
    [Header("UI Bar Fill Images")]
    [Tooltip("Drag the filled image for Health here")]
    public Image healthBarFill;

    [Tooltip("Drag the filled image for Energy here")]
    public Image energyBarFill;

    [Tooltip("Drag the filled image for Stamina here")]
    public Image staminaBarFill;

    [Header("Maximum Stats")]
    public float maxHealth = 100f;
    public float maxEnergy = 100f;
    public float maxStamina = 100f;

    [Header("Current Stats (Read Only visually)")]
    public float currentHealth;
    public float currentEnergy;
    public float currentStamina;

    private void Start()
    {
        // Initialize all stats to maximum on startup
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        currentStamina = maxStamina;

        UpdateAllUIBars();
    }

    /// <summary>
    /// Updates the visual fill amount of all UI bars based on current stats.
    /// FillAmount expects a value between 0 and 1.
    /// </summary>
    public void UpdateAllUIBars()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = currentHealth / maxHealth;

        if (energyBarFill != null)
            energyBarFill.fillAmount = currentEnergy / maxEnergy;

        if (staminaBarFill != null)
            staminaBarFill.fillAmount = currentStamina / maxStamina;
    }

    // --------------------------------------------------------
    // Helper Methods to Modify Stats
    // Call these from your mini-games, shop, or interactions
    // --------------------------------------------------------

    public void ModifyHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateAllUIBars();
    }

    public void ModifyEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
        UpdateAllUIBars();
    }

    public void ModifyStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
        UpdateAllUIBars();
    }
}