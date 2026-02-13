using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    [Header("Stamina UI")]
    public Slider staminaBar;
    public TextMeshProUGUI staminaText;

    [Header("Infection UI")]
    public Slider infectionBar;
    public TextMeshProUGUI infectionText;

    [Header("Level UI")]
    public Slider experienceBar;
    public TextMeshProUGUI levelText;

    [Header("Notifications")]
    public GameObject levelUpPanel;
    public TextMeshProUGUI levelUpText;
    public AudioClip levelUpSound;

    [Header("Death Screen")]
    public GameObject deathPanel;
    public TextMeshProUGUI deathText;
    public Button retryButton;
    public AudioClip deathSound;

    [Header("Colors")]
    public Color healthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public Color staminaColor = Color.blue;
    public Color lowStaminaColor = Color.cyan;
    public Color infectionColor = Color.yellow;
    public Color highInfectionColor = Color.red;

    PlayerStats playerStats;
    PlayerController playerController;
    AudioSource audioSource;

    int lastLevel = 1;

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (deathPanel != null) deathPanel.SetActive(false);

        if (retryButton != null)
            retryButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        if (playerStats == null) return;

        UpdateHealthUI();
        UpdateStaminaUI();
        UpdateInfectionUI();
        UpdateExperienceUI();

        CheckLevelUp();
        CheckPlayerDeath();
    }

    // ================= LEVEL =================

    void CheckLevelUp()
    {
        if (playerStats.level > lastLevel)
        {
            ShowLevelUp();
            lastLevel = playerStats.level;
        }
    }

    void ShowLevelUp()
    {
        if (levelUpPanel == null) return;

        levelUpPanel.SetActive(true);

        if (levelUpText != null)
            levelUpText.text = "NOWY POZIOM!\nPoziom " + playerStats.level;

        if (levelUpSound != null)
            audioSource.PlayOneShot(levelUpSound);

        Invoke(nameof(HideLevelUp), 3f);
    }

    void HideLevelUp()
    {
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }

    // ================= DEATH =================

    void CheckPlayerDeath()
    {
        if (playerStats.currentHealth <= 0 &&
            deathPanel != null &&
            !deathPanel.activeSelf)
        {
            ShowDeath();
        }
    }

    void ShowDeath()
    {
        deathPanel.SetActive(true);

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        Cursor.lockState = CursorLockMode.None;

        if (playerController != null)
            playerController.enabled = false;

        WeaponController wc = playerStats.GetComponent<WeaponController>();

        if (wc != null)
            wc.enabled = false;
    }

    // ================= RESTART =================

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ================= UI UPDATE =================

    void UpdateHealthUI()
    {
        if (healthBar == null) return;

        float percent =
            (float)playerStats.currentHealth /
            playerStats.maxHealth;

        healthBar.value = percent;

        Image img = healthBar.fillRect.GetComponent<Image>();

        if (img != null)
            img.color = Color.Lerp(lowHealthColor, healthColor, percent);

        if (healthText != null)
            healthText.text =
                playerStats.currentHealth +
                " / " +
                playerStats.maxHealth;
    }

    void UpdateStaminaUI()
    {
        if (playerController == null) return;

        if (staminaBar != null)
        {
            float percent =
                playerController.stamina /
                playerController.maxStamina;

            staminaBar.value = percent;

            Image img = staminaBar.fillRect.GetComponent<Image>();

            if (img != null)
                img.color =
                    Color.Lerp(lowStaminaColor, staminaColor, percent);
        }

        if (staminaText != null)
        {
            staminaText.text =
                Mathf.RoundToInt(playerController.stamina) +
                " / " +
                Mathf.RoundToInt(playerController.maxStamina);
        }
    }

    void UpdateInfectionUI()
    {
        if (infectionBar == null) return;

        float percent =
            (float)playerStats.currentInfection /
            playerStats.maxInfection;

        infectionBar.value = percent;

        Image img = infectionBar.fillRect.GetComponent<Image>();

        if (img != null)
            img.color =
                Color.Lerp(infectionColor, highInfectionColor, percent);

        if (infectionText != null)
            infectionText.text =
                "Infection: " +
                playerStats.currentInfection +
                "%";
    }

    void UpdateExperienceUI()
    {
        if (experienceBar != null)
        {
            float percent =
                (float)playerStats.experience /
                playerStats.experienceToNext;

            experienceBar.value = percent;
        }

        if (levelText != null)
        {
            levelText.text =
                "Level " +
                playerStats.level;
        }
    }
}
