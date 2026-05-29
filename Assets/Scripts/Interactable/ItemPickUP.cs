using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Item")]
    public ItemData itemData;

    public int amount = 1;

    [Header("UI")]
    public string promptText = "E - Pick Up";

    public string GetPromptText()
    {
        if (itemData == null)
            return promptText;

        return promptText + " " + itemData.itemName;
    }

    public void Interact()
    {
        if (itemData == null)
            return;

        if (UIManager.Instance != null &&
            UIManager.Instance.IsAnyUIOpen)
            return;

        bool added =
            InventoryManager.Instance.AddItem(
                itemData,
                amount
            );

        if (added)
        {
            Destroy(gameObject);
        }
    }
}