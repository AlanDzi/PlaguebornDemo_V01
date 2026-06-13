using UnityEngine;

public class LevelTransition : MonoBehaviour, IInteractable
{
    [Header("Boss")]
    public GameObject boss;

    [Header("Levels")]
    public GameObject level1Root;
    public GameObject level2Root;

    [Header("Spawn")]
    public Transform level2Spawn;

    public string GetPromptText()
    {
        if (boss == null)
            return "E - Go Deeper";

        return "Defeat the Boss";
    }

    public void Interact()
    {
        if (boss != null)
            return;

        // W³¹cz kolejny level
        level2Root.SetActive(true);

        // Wy³¹cz stary level
        level1Root.SetActive(false);

        // Teleport gracza
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null &&
            level2Spawn != null)
        {
            player.transform.position =
                level2Spawn.position;
        }
    }
}