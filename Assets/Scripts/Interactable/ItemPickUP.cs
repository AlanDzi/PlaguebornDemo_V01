using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public string promptText = "E - podnieœ";

    public ChestItemType itemType;
    public int amount = 1;

    private InventoryManager inventory;

    void Start()
    {
        inventory = FindFirstObjectByType<InventoryManager>();
    }

    public string GetPromptText()
    {
        return promptText;
    }

    public void Interact()
    {
        if (inventory == null) return;

        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
            return;

        bool added = false;

        switch (itemType)
        {
            case ChestItemType.Bandage:
                added = inventory.AddBandage(amount);
                break;

            case ChestItemType.Antidote:
                added = inventory.AddAntidote(amount);
                break;

            case ChestItemType.Coins:
                inventory.AddCoins(amount);
                added = true;
                break;
        }

        if (!added) return;

        Destroy(gameObject);
    }
}