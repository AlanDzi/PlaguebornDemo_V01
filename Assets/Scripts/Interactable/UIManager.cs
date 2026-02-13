using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Note")]
    public CanvasGroup noteGroup;
    public TMP_Text noteText;

    [Header("Prompt")]
    public GameObject interactionPrompt;
    public TMP_Text interactionPromptText;

    [Header("Chest")]
    public GameObject chestPanel;
    public Transform chestContent;
    public GameObject chestItemButtonPrefab;

    [Header("End Game")]
    public GameObject endGamePanel;

    private bool isNoteOpen;
    private bool isChestOpen;
    private bool isEndGameOpen;

    private float blockInteractUntil = 0f;
    public bool CanInteractNow => Time.time >= blockInteractUntil;

    private PlayerInteract playerInteract;

    public bool IsAnyUIOpen => isNoteOpen || isChestOpen || isEndGameOpen;

    private void Awake()
    {
        Instance = this;

        playerInteract = FindFirstObjectByType<PlayerInteract>();

        blockInteractUntil = 0f;

        if (noteGroup != null)
        {
            noteGroup.alpha = 0;
            noteGroup.interactable = false;
            noteGroup.blocksRaycasts = false;
        }

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (chestPanel != null)
            chestPanel.SetActive(false);

        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        isNoteOpen = false;
        isChestOpen = false;
        isEndGameOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isNoteOpen)
            {
                CloseNote();
                return;
            }

            if (isChestOpen)
            {
                CloseChest();
                return;
            }
        }
    }

    // ================= NOTE =================

    public void ShowNote(string text)
    {
        isNoteOpen = true;

        noteText.text = text;

        noteGroup.alpha = 1;
        noteGroup.interactable = true;
        noteGroup.blocksRaycasts = true;

        LockPlayer(true);
    }

    public void CloseNote()
    {
        isNoteOpen = false;

        noteGroup.alpha = 0;
        noteGroup.interactable = false;
        noteGroup.blocksRaycasts = false;

        blockInteractUntil = Time.time + 0.2f;

        LockPlayer(false);
    }

    // ================= CHEST =================

    public void ShowChest(Chest chest)
    {
        isChestOpen = true;

        chestPanel.SetActive(true);
        interactionPrompt.SetActive(false);

        LockPlayer(true);

        RefreshChest(chest);
    }

    public void CloseChest()
    {
        isChestOpen = false;

        chestPanel.SetActive(false);

        blockInteractUntil = Time.time + 0.2f;

        LockPlayer(false);
    }

    public void RefreshChest(Chest chest)
    {
        foreach (Transform t in chestContent)
            Destroy(t.gameObject);

        foreach (var item in chest.items)
        {
            Instantiate(chestItemButtonPrefab, chestContent)
                .GetComponent<ChestItemButton>()
                .Setup(item, chest);
        }
    }

    // ================= END GAME =================

    public void ShowEndGame()
    {
        isEndGameOpen = true;

        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        LockPlayer(true);

        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ================= PROMPT =================

    public void ShowInteractionPrompt(bool show, string text = "")
    {
        if (IsAnyUIOpen) return;

        interactionPrompt.SetActive(show);

        if (!string.IsNullOrEmpty(text))
            interactionPromptText.text = text;
    }

    // ================= PLAYER =================

    void LockPlayer(bool lockPlayer)
    {
        if (playerInteract != null)
            playerInteract.enabled = !lockPlayer;

        if (lockPlayer)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
