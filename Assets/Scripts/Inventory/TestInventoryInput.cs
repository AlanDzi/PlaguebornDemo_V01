using UnityEngine;

public class TestInventoryInput : MonoBehaviour
{
    public ItemData bandageItem;
    public ItemData antidoteItem;
    public ItemData pistolItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            InventoryManager.Instance.AddItem(
                bandageItem,
                1
            );
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            InventoryManager.Instance.AddItem(
                antidoteItem,
                1
            );
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            InventoryManager.Instance.AddItem(
                pistolItem,
                1
            );
        }
    }
}