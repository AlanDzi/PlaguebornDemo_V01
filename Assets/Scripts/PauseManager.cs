using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;

    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Button backFromSettingsButton;
    public Slider sensitivitySlider;
    public Slider volumeSlider;

    [Header("Confirm Panel")]
    public GameObject confirmPanel;
    public TextMeshProUGUI confirmText;
    public Button yesButton;
    public Button noButton;

    [Header("Audio")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;

    private bool isPaused = false;
    private AudioSource audioSource;
    private PlayerController playerController;
    private WeaponController weaponController;

    public static bool IsPausedStatic;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        playerController = FindFirstObjectByType<PlayerController>();
        weaponController = FindFirstObjectByType<WeaponController>();

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);

        SetupButtons();
        SetupSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (confirmPanel != null && confirmPanel.activeSelf)
            {
                HideConfirmPanel();
            }
            else if (settingsPanel != null && settingsPanel.activeSelf)
            {
                HideSettings();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // ================= BUTTONS =================

    void SetupButtons()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
            AddButtonEffects(resumeButton);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(ShowSettings);
            AddButtonEffects(settingsButton);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ShowMainMenuConfirm);
            AddButtonEffects(mainMenuButton);
        }

        if (backFromSettingsButton != null)
        {
            backFromSettingsButton.onClick.AddListener(HideSettings);
            AddButtonEffects(backFromSettingsButton);
        }

        if (yesButton != null)
        {
            yesButton.onClick.AddListener(ReturnToMainMenu);
            AddButtonEffects(yesButton);
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(HideConfirmPanel);
            AddButtonEffects(noButton);
        }
    }

    // ================= SETTINGS =================

    void SetupSettings()
    {
        float sens = PlayerPrefs.GetFloat("Sensitivity", 2f);
        float vol = PlayerPrefs.GetFloat("Volume", 1f);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = sens;
            sensitivitySlider.onValueChanged.AddListener(ApplySensitivity);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = vol;
            volumeSlider.onValueChanged.AddListener(ApplyVolume);
        }

        ApplySensitivity(sens);
        ApplyVolume(vol);
    }

    public void ApplySensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);

        if (playerController != null)
            playerController.mouseSensitivity = value;
    }

    public void ApplyVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void HideSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // ================= EFFECTS =================

    void AddButtonEffects(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((data) => OnButtonHover(button));
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) => OnButtonExit(button));
        trigger.triggers.Add(exit);

        button.onClick.AddListener(PlayButtonClickSound);
    }

    void OnButtonHover(Button button)
    {
        if (buttonHoverSound != null)
            audioSource.PlayOneShot(buttonHoverSound, 0.5f);

        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            text.color = new Color(0.15f, 0.08f, 0f);
    }

    void OnButtonExit(Button button)
    {
        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            text.color = new Color(0.35f, 0.2f, 0.1f);
    }

    void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }

    // ================= GAME STATE =================

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
            playerController.enabled = false;

        if (weaponController != null)
            weaponController.enabled = false;

        IsPausedStatic = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
            playerController.enabled = true;

        if (weaponController != null)
            weaponController.enabled = true;

        IsPausedStatic = false;
    }

    // ================= CONFIRM =================

    public void ShowMainMenuConfirm()
    {
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(true);
            if (confirmText != null)
                confirmText.text = "Czy na pewno?";
        }
    }

    public void HideConfirmPanel()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}