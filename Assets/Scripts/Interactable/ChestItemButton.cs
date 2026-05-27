using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChestItemButton : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text amountText;
    public Button button;

    private ChestItem item;
    private Chest chest;

    public void Setup(
        ChestItem newItem,
        Chest newChest
    )
    {
        item = newItem;
        chest = newChest;

        if (
            item == null ||
            item.itemData == null
        )
        {
            gameObject.SetActive(false);
            return;
        }

        if (icon != null)
        {
            icon.sprite =
                item.itemData.icon;

            icon.enabled = true;
        }

        if (amountText != null)
        {
            amountText.text =
                item.amount > 1
                ? item.amount.ToString()
                : "";
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (item == null)
            return;

        bool added =
            InventoryManager.Instance.AddItem(
                item.itemData,
                item.amount
            );

        if (!added)
            return;

        chest.TakeItem(item);
    }
}