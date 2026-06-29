using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Main")]
    public GameObject inventoryPanel;

    [Header("Inventory")]
    public Transform inventoryGrid;
    public GameObject slotPrefab;

    [Header("Weapon")]
    public InventorySlotUI weaponSlotUI;

    [Header("Hotbar")]
    public InventorySlotUI[] hotbarSlots;

    private InventoryManager inventory;

    private bool isOpen = false;
    public bool IsOpen => isOpen;

    void Start()
    {
        Debug.Log("InventoryUI START");

        inventory = InventoryManager.Instance;

        CreateInventorySlots();
        
        SetupHotbarSlots();

        CloseInventory();
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab))
            return;

        // mo¿na zawsze zamkn¹æ inventory
        if (isOpen)
        {
            CloseInventory();
            return;
        }

        // nie otwieraj jeœli jest otwarte inne UI
        if (UIManager.Instance != null &&
            UIManager.Instance.IsAnyUIOpen)
        {
            return;
        }

        // nie otwieraj podczas pauzy
        if (PauseManager.IsPausedStatic)
        {
            return;
        }

        OpenInventory();
    }

    // ================= OPEN / CLOSE =================

    public void OpenInventory()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideChest();
        }

        isOpen = true;

        inventoryPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        RefreshInventory();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetInventoryState(true);
        }
    }

    public void CloseInventory()
    {
        isOpen = false;

        inventoryPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetInventoryState(false);

        }

        if (ItemTooltipUI.Instance != null)
        {
            ItemTooltipUI.Instance.Hide();
        }
    }

    // ================= CREATE =================

    void CreateInventorySlots()

    {
        Debug.Log("CREATING SLOTS");

        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            GameObject obj =
                Instantiate(
                    slotPrefab,
                    inventoryGrid
                );

            InventorySlotUI slotUI =
                obj.GetComponent<InventorySlotUI>();

            slotUI.slotIndex = i;

            slotUI.inventoryUI = this;
        }
    }

    // ================= REFRESH =================

    public void RefreshInventory()
    {
        if (inventory == null)
            return;

        // INVENTORY GRID
        if (inventoryGrid != null)
        {
            InventorySlotUI[] slots =
                inventoryGrid.GetComponentsInChildren<InventorySlotUI>(true);

            for (int i = 0; i < slots.Length; i++)
            {
                if (i >= inventory.inventorySlots.Length)
                    break;

                if (slots[i] != null)
                {
                    slots[i].SetSlot(
                        inventory.inventorySlots[i]
                    );
                }
            }
        }

        // HOTBAR
        if (hotbarSlots != null &&
            inventory.hotbarSlots != null)
        {
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (i >= inventory.hotbarSlots.Length)
                    break;

                if (hotbarSlots[i] != null)
                {
                    hotbarSlots[i].SetSlot(
                        inventory.hotbarSlots[i]
                    );
                }
            }
        }

        // WEAPON SLOT
        if (weaponSlotUI != null &&
            inventory.equippedWeapon != null)
        {
            weaponSlotUI.SetSlot(
                inventory.equippedWeapon
            );
        }
    }

    void SetupHotbarSlots()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].slotIndex = 100 + i;
            hotbarSlots[i].inventoryUI = this;
        }

        if (weaponSlotUI != null)
        {
            weaponSlotUI.slotIndex = 200;
            weaponSlotUI.inventoryUI = this;
        }
    }
}