using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Victory Settings")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryText;
    public Button nextLevelButton;
    public Button mainMenuButton;

    [Header("Audio")]
    public AudioClip victorySound;

    [Header("Level Settings")]
    public string nextLevelScene = "TestScene2";
    public string mainMenuScene = "MainMenuScene";

    [Header("Debug / Testing")]
    [Tooltip("Zaznaczone = poziom NIE kończy się po zabiciu wrogów")]
    public bool disableVictory = true; // <<< TERAZ WYŁĄCZONE

    private Enemy[] allEnemies;
    private AudioSource audioSource;
    private bool victoryTriggered = false;
    private bool isLastLevel = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        FindAllEnemies();
        CheckPlayerClass();

        string currentScene = SceneManager.GetActiveScene().name;
        isLastLevel = (currentScene == "TestScene2");

        if (nextLevelButton != null)
        {
            TextMeshProUGUI buttonText =
                nextLevelButton.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text =
                    isLastLevel ? "Powrót do menu" : "Następny poziom";
        }

        if (victoryText != null)
        {
            victoryText.text =
                isLastLevel ? "To Koniec" : "Zwycięstwo!";
        }

        SetupButtons();
    }

    // ================= BUTTONS =================

    void SetupButtons()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(() =>
            {
                LoadNextLevel();
            });
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                LoadMainMenu();
            });
        }
    }

    // ================= UPDATE =================

    void Update()
    {
        // WYŁĄCZONE ZWYCIĘSTWO
        if (disableVictory) return;

        if (!victoryTriggered)
        {
            CheckVictoryCondition();
        }
    }

    // ================= ENEMIES =================

    void FindAllEnemies()
    {
        allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        System.Collections.Generic.List<Enemy> list =
            new System.Collections.Generic.List<Enemy>(allEnemies);

        list.Add(enemy);
        allEnemies = list.ToArray();
    }

    // ================= PLAYER CLASS =================

    void CheckPlayerClass()
    {
        string selectedClass =
            PlayerPrefs.GetString("SelectedClass", "");

        if (!string.IsNullOrEmpty(selectedClass))
        {
            // Na przyszłość: klasy postaci
        }
    }

    // ================= VICTORY =================

    void CheckVictoryCondition()
    {
        int aliveEnemies = 0;

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy != null)
                aliveEnemies++;
        }

        if (aliveEnemies == 0 && allEnemies.Length > 0)
        {
            TriggerVictory();
        }
    }

    void TriggerVictory()
    {
        // PODWÓJNE ZABEZPIECZENIE
        if (disableVictory) return;

        victoryTriggered = true;

        PlayerStats playerStats =
            FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
            playerStats.OnGameWon();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerController pc =
            FindFirstObjectByType<PlayerController>();

        if (pc != null)
            pc.enabled = false;

        WeaponController wc =
            FindFirstObjectByType<WeaponController>();

        if (wc != null)
            wc.enabled = false;

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (victorySound != null)
            audioSource.PlayOneShot(victorySound);
    }

    // ================= SCENES =================

    public void LoadNextLevel()
    {
        SaveSystem.SavePlayerData();

        if (isLastLevel)
        {
            SceneManager.LoadScene(mainMenuScene);
        }
        else
        {
            SceneManager.LoadScene(nextLevelScene);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    // ================= EXTERNAL CALL =================

    public void CheckForVictory()
    {
        if (disableVictory) return;

        if (!victoryTriggered)
        {
            CheckVictoryCondition();
        }
    }
}
