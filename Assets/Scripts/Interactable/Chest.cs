using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Chest Items")]
    public List<ChestItem> items = new List<ChestItem>();

    [Header("UI")]
    public string promptText = "E - Otwórz skrzyniê";

    public string GetPromptText()
    {
        return promptText;
    }

    public void Interact()
    {
        if (UIManager.Instance == null)
            return;

        if (UIManager.Instance.IsAnyUIOpen)
            return;

        UIManager.Instance.ShowChest(this);
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