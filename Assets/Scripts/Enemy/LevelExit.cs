using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit :
    MonoBehaviour,
    IInteractable
{
    [Header("Boss do pokonania")]
    public GameObject boss;

    [Header("Prompt")]
    public string lockedPrompt =
        "Pokonaj bossa";

    public string openPrompt =
        "E - Zejdü niøej";

    public string GetPromptText()
    {
        if (boss == null)
            return openPrompt;

        return lockedPrompt;
    }

    public void Interact()
    {
        if (boss != null)
            return;

        LoadNextLevel();
    }

    void LoadNextLevel()
    {
        int currentIndex =
            SceneManager
                .GetActiveScene()
                .buildIndex;

        SceneManager.LoadScene(
            currentIndex + 1
        );
    }
}