using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door")]
    public float openAngle = 90f;
    public float speed = 3f;

    [Header("Lock")]
    public bool requiresKey = false;
    public ItemData requiredKey;
    public bool consumeKey = true;

    [Header("Teleport (Optional)")]
    public Transform teleportTarget;

    [Header("Boss Spawn")]
    public GameObject bossToActivate;

    [Header("Prompt")]
    public string openText = "E - Open/Close";
    public string lockedText = "Key required";

    private bool isOpen = false;

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = transform.localRotation;
        openRot = Quaternion.Euler(0, openAngle, 0) * closedRot;
    }

    void Update()
    {
        Quaternion target =
            isOpen ? openRot : closedRot;

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            target,
            Time.deltaTime * speed
        );
    }

    public string GetPromptText()
    {
        if (requiresKey)
        {
            if (!HasKey())
                return lockedText;
        }

        return openText;
    }

    public void Interact()
    {
        if (UIManager.Instance != null &&
            UIManager.Instance.IsAnyUIOpen)
            return;

        if (requiresKey)
        {
            if (!HasKey())
                return;

            if (consumeKey)
            {
                RemoveKey();
            }

            requiresKey = false;
        }

        isOpen = !isOpen;

        if (teleportTarget != null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                player.transform.position =
                    teleportTarget.position;
            }
            if (bossToActivate != null)
            {
                bossToActivate.SetActive(true);
            }
        }
    }

    bool HasKey()
    {
        InventoryManager inv =
            InventoryManager.Instance;

        if (inv == null)
            return false;

        foreach (InventorySlot slot in inv.inventorySlots)
        {
            if (!slot.IsEmpty() &&
                slot.item == requiredKey)
            {
                return true;
            }
        }

        return false;
    }

    void RemoveKey()
    {
        InventoryManager inv =
            InventoryManager.Instance;

        if (inv == null)
            return;

        for (int i = 0; i < inv.inventorySlots.Length; i++)
        {
            InventorySlot slot =
                inv.inventorySlots[i];

            if (!slot.IsEmpty() &&
                slot.item == requiredKey)
            {
                slot.Clear();

                inv.RefreshUI();

                return;
            }
        }
    }
}