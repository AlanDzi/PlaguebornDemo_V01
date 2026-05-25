using UnityEngine;
using UnityEngine.UI;

public class InventoryDragHandler : MonoBehaviour
{
    public static InventoryDragHandler Instance;

    [Header("UI")]
    public Image dragIcon;

    private InventorySlot draggedSlot;
    private int draggedFromIndex = -1;

    private bool isDragging = false;

    private int selectedSlotIndex = -1;

    private InventorySlotUI draggedSlotUI;

    private InventorySlotUI selectedSlotUI;

    void Awake()
    {
        Instance = this;

        dragIcon.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isDragging)
            return;

        RectTransform rect =
            dragIcon.rectTransform;

        rect.position = Input.mousePosition;
    }

    // ================= START DRAG =================

    public void StartDrag(
        InventorySlot slot,
        int fromIndex,
        InventorySlotUI slotUI
    )
    {
        if (slot == null || slot.IsEmpty())
            return;

        draggedSlot = slot;

        draggedFromIndex = fromIndex;

        draggedSlotUI = slotUI;

        isDragging = true;

        dragIcon.gameObject.SetActive(true);

        dragIcon.enabled = true;

        dragIcon.sprite = slot.item.icon;
    }

    // ================= DROP =================

    public void DropOnSlot(
    int targetIndex,
    InventorySlotUI targetSlotUI
)
    {
        if (!isDragging || draggedSlot == null)
            return;

        InventoryManager inv =
            InventoryManager.Instance;

        InventorySlot fromSlot = GetSlotByUI(draggedFromIndex, draggedSlotUI);

        InventorySlot toSlot = GetSlotByUI(targetIndex, targetSlotUI);

        if (fromSlot == null || toSlot == null)
        {
            StopDrag();
            return;
        }

        // weapon accepts only weapon
        if (
            targetSlotUI.slotType == InventorySlotType.Weapon &&
            fromSlot.item != null &&
            fromSlot.item.itemType != ItemType.Weapon
        )
        {
            StopDrag();
            return;
        }

        // hotbar accepts only consumables
        if (
            targetSlotUI.slotType == InventorySlotType.Hotbar &&
            fromSlot.item != null &&
            fromSlot.item.itemType != ItemType.Consumable
        )
        {
            StopDrag();
            return;
        }

        // swap
        SetSlotByUI(targetIndex, targetSlotUI, fromSlot);
        SetSlotByUI(draggedFromIndex, draggedSlotUI, toSlot);

        inv.RefreshUI();
        inv.UpdateEquippedWeapon();

        StopDrag();
        RefreshSelectionVisual();
    }

    // ================= CLICK MOVE =================

    public void ClickSlot(
    int clickedIndex,
    InventorySlotUI clickedSlotUI
)
    {
        InventoryManager inv =
            InventoryManager.Instance;

        InventorySlot clickedSlot =
            GetSlotByUI(
                clickedIndex,
                clickedSlotUI
            );

        // FIRST CLICK
        if (selectedSlotIndex == -1)
        {
            if (clickedSlot == null ||
                clickedSlot.IsEmpty())
                return;

            selectedSlotIndex = clickedIndex;
            selectedSlotUI = clickedSlotUI;

            draggedFromIndex = clickedIndex;
            draggedSlotUI = clickedSlotUI;
            draggedSlot = clickedSlot;

            // wa¿ne:
            isDragging = true;

            RefreshSelectionVisual();


            return;

            void Update()
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    DropSelectedItem();
                }
            }
        }

        // klikniêcie drugi raz tego samego
        if (
            selectedSlotIndex == clickedIndex &&
            draggedSlotUI == clickedSlotUI
        )
        {
            selectedSlotIndex = -1;

            draggedSlot = null;
            draggedSlotUI = null;
            draggedFromIndex = -1;

            isDragging = false;

            RefreshSelectionVisual();

            return;
        }

        // SECOND CLICK
        DropOnSlot(
            clickedIndex,
            clickedSlotUI
        );

        selectedSlotIndex = -1;

        draggedSlot = null;
        draggedSlotUI = null;
        draggedFromIndex = -1;

        isDragging = false;

        RefreshSelectionVisual();
    }

    // ================= VISUAL =================

    void RefreshSelectionVisual()
    {
        InventorySlotUI[] slots =
            FindObjectsByType<InventorySlotUI>(
                FindObjectsSortMode.None
            );

        foreach (InventorySlotUI slot in slots)
        {
            bool selected =
                slot.slotIndex ==
                selectedSlotIndex;

            slot.SetSelected(selected);
        }
    }

    // ================= STOP =================

    public void StopDrag()
    {
        isDragging = false;

        dragIcon.enabled = false;

        dragIcon.gameObject.SetActive(false);

        dragIcon.sprite = null;

        draggedSlot = null;

        draggedFromIndex = -1;

        draggedSlotUI = null;
    }
    InventorySlot GetSlotByUI(
    int index,
    InventorySlotUI slotUI
)
    {
        InventoryManager inv =
            InventoryManager.Instance;

        if (slotUI == null)
            return null;

        switch (slotUI.slotType)
        {
            case InventorySlotType.Inventory:
                return inv.inventorySlots[index];

            case InventorySlotType.Hotbar:
                return inv.hotbarSlots[index - 100];

            case InventorySlotType.Weapon:
                return inv.equippedWeapon;
        }

        return null;
    }

    void SetSlotByUI(
        int index,
        InventorySlotUI slotUI,
        InventorySlot slot
    )
    {
        InventoryManager inv =
            InventoryManager.Instance;

        if (slotUI == null)
            return;

        switch (slotUI.slotType)
        {
            case InventorySlotType.Inventory:
                inv.inventorySlots[index] = slot;
                break;

            case InventorySlotType.Hotbar:
                inv.hotbarSlots[index - 100] = slot;
                break;

            case InventorySlotType.Weapon:
                inv.equippedWeapon = slot;
                break;
        }
    }
    public void DropSelectedItem()
    {
        if (selectedSlotIndex == -1 ||
            selectedSlotUI == null)
            return;

        InventorySlot slot =
            GetSlotByUI(
                selectedSlotIndex,
                selectedSlotUI
            );

        if (slot == null ||
            slot.IsEmpty() ||
            slot.item == null)
            return;

        if (slot.item.worldPrefab != null)
        {
            PlayerController player =
                FindFirstObjectByType<PlayerController>();

            if (player != null)
            {
                Vector3 spawnPos =
                    player.transform.position +
                    player.transform.forward * 1.5f +
                    Vector3.up * 0.3f;

                Instantiate(
                    slot.item.worldPrefab,
                    spawnPos,
                    Quaternion.identity
                );
            }
        }

        slot.amount--;

        if (slot.amount <= 0)
        {
            slot.Clear();
        }

        InventoryManager.Instance.RefreshUI();

        selectedSlotIndex = -1;
        selectedSlotUI = null;

        RefreshSelectionVisual();
    }
}