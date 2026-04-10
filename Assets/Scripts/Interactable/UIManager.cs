using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
   
    [Header("Skill Tree")]
    public GameObject skillTreePanel;
    private bool isSkillTreeOpen;

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

    

    [Header("UI Sounds")]
    public AudioClip noteOpenSound;
    public AudioClip noteCloseSound;

    public AudioClip chestOpenSound;
    public AudioClip chestCloseSound;

    public AudioSource uiAudioSource;

    

    private bool isNoteOpen;
    private bool isChestOpen;
    private bool isEndGameOpen;

    private float blockInteractUntil = 0f;
    public bool CanInteractNow => Time.time >= blockInteractUntil;

    private PlayerInteract playerInteract;

    public bool IsAnyUIOpen => isNoteOpen || isChestOpen || isEndGameOpen || isSkillTreeOpen;

    private void Awake()
    {
        Instance = this;

        playerInteract = FindFirstObjectByType<PlayerInteract>();

        blockInteractUntil = 0f;

      
        if (uiAudioSource == null)
            uiAudioSource = GetComponent<AudioSource>();

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

        if (skillTreePanel != null)
            skillTreePanel.SetActive(false);

        isSkillTreeOpen = false;
        isNoteOpen = false;
        isChestOpen = false;
        isEndGameOpen = false;
    }

    void Update()
    {
        // OBS£UGA E (note / chest)
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

        // OBS£UGA P (skill tree)  MUSI BYÆ POZA E
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isSkillTreeOpen)
            {
                CloseSkillTree();
            }
            else if (!IsAnyUIOpen)
            {
                OpenSkillTree();
            }
        }
    }




    public void ShowNote(string text)
    {
        
        if (uiAudioSource != null && noteOpenSound != null)
            uiAudioSource.PlayOneShot(noteOpenSound);

        isNoteOpen = true;

        noteText.text = text;

        noteGroup.alpha = 1;
        noteGroup.interactable = true;
        noteGroup.blocksRaycasts = true;

        LockPlayer(true);
    }

    public void CloseNote()
    {
       
        if (uiAudioSource != null && noteCloseSound != null)
            uiAudioSource.PlayOneShot(noteCloseSound);

        isNoteOpen = false;

        noteGroup.alpha = 0;
        noteGroup.interactable = false;
        noteGroup.blocksRaycasts = false;

        blockInteractUntil = Time.time + 0.2f;

        LockPlayer(false);
    }

   

    public void ShowChest(Chest chest)
    {
        if (uiAudioSource != null && chestOpenSound != null)
            uiAudioSource.PlayOneShot(chestOpenSound);

        isChestOpen = true;

        chestPanel.SetActive(true);
        interactionPrompt.SetActive(false);

        LockPlayer(true);

        RefreshChest(chest);
    }

    public void CloseChest()
    {
        
        if (uiAudioSource != null && chestCloseSound != null)
            uiAudioSource.PlayOneShot(chestCloseSound);

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

    

    public void ShowInteractionPrompt(bool show, string text = "")
    {
        if (IsAnyUIOpen) return;

        interactionPrompt.SetActive(show);

        if (!string.IsNullOrEmpty(text))
            interactionPromptText.text = text;
    }

    

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
    // ================= SKILL TREE =================

    public void OpenSkillTree()
    {
        if (skillTreePanel == null) return;

        isSkillTreeOpen = true;

        skillTreePanel.SetActive(true);

        LockPlayer(true);

        Time.timeScale = 0f;
    }

    public void CloseSkillTree()
    {
        if (skillTreePanel == null) return;

        isSkillTreeOpen = false;

        skillTreePanel.SetActive(false);

        blockInteractUntil = Time.time + 0.2f;

        LockPlayer(false);

        // tylko jeœli NIC innego nie jest otwarte
        if (!IsAnyUIOpen)
            Time.timeScale = 1f;
    }

}
