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

        // ZABEZPIECZENIE
        if (label == null)
        {
            Debug.LogError("ChestItemButton: LABEL NOT ASSIGNED!");
            return;
        }

        if (button == null)
        {
            Debug.LogError("ChestItemButton: BUTTON NOT ASSIGNED!");
            return;
        }

        label.text = item.type + " x" + item.amount;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (chest != null && item != null)
            chest.TakeItem(item);
    }
}
