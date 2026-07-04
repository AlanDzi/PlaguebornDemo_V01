using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject confirmPanel;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Info Panel")]
    public GameObject infoPanel;
    public Button infoButton;
    public Button closeInfoButton;

    [Header("Settings")]
    public Button backFromSettingsButton;

    [Header("Quit Confirm")]
    public TextMeshProUGUI confirmText;
    public Button yesButton;
    public Button noButton;

    [Header("Audio")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;
    public AudioClip ambientMusic;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMainMenu();
        SetupButtons();
        SetupAmbientMusic();
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    void SetupButtons()
    {
        playButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(ShowSettings);
        quitButton.onClick.AddListener(ShowQuitConfirmation);

        backFromSettingsButton.onClick.AddListener(ShowMainMenu);

        yesButton.onClick.AddListener(QuitGame);
        noButton.onClick.AddListener(ShowMainMenu);

        AddButtonEffects(playButton);
        AddButtonEffects(settingsButton);
        AddButtonEffects(quitButton);
        AddButtonEffects(backFromSettingsButton);
        AddButtonEffects(yesButton);
        AddButtonEffects(noButton);
       
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(ShowInfoPanel);
            AddButtonEffects(infoButton);
        }

        if (closeInfoButton != null)
        {
            closeInfoButton.onClick.AddListener(HideInfoPanel);
            AddButtonEffects(closeInfoButton);
        }
    }

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
        {
            text.color = new Color(0.15f, 0.08f, 0f); // ciemny "ink"
        }

        button.transform.localScale = Vector3.one * 1.05f;
    }

    void OnButtonExit(Button button)
    {
        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = new Color(0.35f, 0.2f, 0.1f); // brąz papierowy
        }

        button.transform.localScale = Vector3.one;
    }

    void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        confirmPanel.SetActive(false);
    }

    public void ShowQuitConfirmation()
    {
        confirmPanel.SetActive(true);
        confirmText.text = "Are you sure you want to quit?";
    }

    public void StartGame()
    {
        SceneManager.LoadScene("ComicIntro");
    }

    public void ShowInfoPanel()
    {
        mainMenuPanel.SetActive(false);

        if (infoPanel != null)
            infoPanel.SetActive(true);
    }

    public void HideInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);

        ShowMainMenu();
    }


    void SetupAmbientMusic()
    {
        if (ambientMusic != null)
        {
            audioSource.clip = ambientMusic;
            audioSource.loop = true;
            audioSource.volume = 0.4f;
            audioSource.Play();
        }
    }

    public void QuitGame()
    {
        SaveSystem.SavePlayerData();

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}