using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Player Progress")]
    public int playerLevel = 1;
    public int playerExperience = 0;
    public int playerExperienceToNext = 100;

    public int playerHealth = 100;
    public int playerMaxHealth = 100;

    public int playerInfection = 0;

    public int playerDamage = 20;

    [Header("First Level Setup")]
    public bool isFirstLevel = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ================= SAVE =================

    public void SavePlayerData()
    {
        PlayerStats playerStats =
            FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
        {
            playerLevel = playerStats.level;

            playerExperience =
                playerStats.experience;

            playerExperienceToNext =
                playerStats.experienceToNext;

            playerHealth =
                playerStats.currentHealth;

            playerMaxHealth =
                playerStats.maxHealth;

            playerInfection =
                playerStats.currentInfection;

            playerDamage =
                playerStats.currentDamage;
        }

        isFirstLevel = false;
    }

    // ================= LOAD =================

    public void LoadPlayerData()
    {
        if (isFirstLevel)
            return;

        PlayerStats playerStats =
            FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.level =
                playerLevel;

            playerStats.experience =
                playerExperience;

            playerStats.experienceToNext =
                playerExperienceToNext;

            playerStats.currentHealth =
                playerHealth;

            playerStats.maxHealth =
                playerMaxHealth;

            playerStats.currentInfection =
                playerInfection;

            playerStats.currentDamage =
                playerDamage;
        }
    }

    // ================= RESET =================

    public void ResetData()
    {
        playerLevel = 1;

        playerExperience = 0;

        playerExperienceToNext = 100;

        playerHealth = 100;

        playerMaxHealth = 100;

        playerInfection = 0;

        playerDamage = 20;

        isFirstLevel = true;
    }
}