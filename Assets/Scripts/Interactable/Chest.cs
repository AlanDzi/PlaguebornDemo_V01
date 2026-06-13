using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Chest Items")]
    public List<ChestItem> items = new List<ChestItem>();

    [Header("UI")]
    public string promptText = "E - Open";

    [Header("Boss Requirement")]
    public GameObject requiredBoss;

    public string GetPromptText()
    {
        if (requiredBoss != null)
        {
            return "Defeat the Boss!";
        }

        return promptText;
    }

    public void Interact()
    {
        if (requiredBoss != null)
            return;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowChest(this);
        }
    }

    public void TakeItem(ChestItem item)
    {
        if (item == null)
            return;

        items.Remove(item);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshChest(this);
        }
    }
}