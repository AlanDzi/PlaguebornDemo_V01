using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ChestItemButton : MonoBehaviour
{
    public TMP_Text label;
    public Button button;

    private ChestItem item;
    private Chest chest;

    public void Setup(ChestItem newItem, Chest newChest)
    {
        item = newItem;
        chest = newChest;

        if (label == null || button == null)
            return;

        label.text =
            item.itemData.itemName +
            " x" +
            item.amount;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
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