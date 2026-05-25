using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IPointerClickHandler
{
    [Header("UI")]
    public Image icon;
    public TextMeshProUGUI amountText;

    [HideInInspector]
    public int slotIndex;

    [HideInInspector]
    public InventoryUI inventoryUI;

    private InventorySlot currentSlot;


    public Image selectionBorder;
   
    [Header("Slot Type")]
    public InventorySlotType slotType;
    public void SetSlot(InventorySlot slot)
    {
        currentSlot = slot;

        if (slot == null || slot.IsEmpty())
        {
            icon.enabled = false;
            amountText.text = "";

            return;
        }

        icon.enabled = true;
        icon.sprite = slot.item.icon;

        if (slot.item.stackable)
        {
            amountText.text = slot.amount.ToString();
        }
        else
        {
            amountText.text = "";
        }
    }

   

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.IsEmpty())
            return;

        InventoryDragHandler.Instance.StartDrag(
            currentSlot,
            slotIndex,
            this
        );
    }


    public void OnDrag(PointerEventData eventData)
    {
        // Required by Unity drag system
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryDragHandler.Instance != null)
        {
            InventoryDragHandler.Instance.StopDrag();
        }
    }



    public void OnDrop(PointerEventData eventData)
    {
        InventoryDragHandler.Instance.DropOnSlot(
            slotIndex,
            this
        );
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button ==
            PointerEventData.InputButton.Left)
        {
            InventoryDragHandler.Instance
     .ClickSlot(
         slotIndex,
         this
     );
        }
    }

    public void SetSelected(bool selected)
    {
        selectionBorder.enabled = selected;
    }
    void Start()
    {
        selectionBorder.enabled = false;
    }
}