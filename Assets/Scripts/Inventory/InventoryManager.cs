using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory")]
    public int inventorySize = 24;

    public InventorySlot[] inventorySlots;

    [Header("Hotbar")]
    public InventorySlot[] hotbarSlots = new InventorySlot[4];

    [Header("Weapon")]
    public InventorySlot equippedWeapon = new InventorySlot();

    [Header("Audio")]
    public AudioClip useSound;

    [Header("Starting Equipment")]
    public ItemData startingWeapon;

    private AudioSource audioSource;
    private PlayerStats playerStats;

    void Awake()
    {

        Debug.Log("InventoryManager Awake");


        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        inventorySlots = new InventorySlot[inventorySize];

        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots[i] = new InventorySlot();
        }

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i] = new InventorySlot();
        }
    }

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (equippedWeapon == null ||
    equippedWeapon.IsEmpty())
        {
            if (startingWeapon != null)
            {
                equippedWeapon =
                    new InventorySlot(
                        startingWeapon,
                        1
                    );
            }
        }
        UpdateEquippedWeapon();
        RefreshUI();
    }

    void Update()
    {
        if (UIManager.Instance != null &&
            UIManager.Instance.IsAnyUIOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            UseHotbarSlot(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            UseHotbarSlot(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            UseHotbarSlot(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            UseHotbarSlot(3);
    }

    // ================= ADD ITEM =================

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null)
            return false;

        // STACK
        if (item.stackable)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];

                if (!slot.IsEmpty() &&
                    slot.item == item &&
                    slot.amount < item.maxStack)
                {
                    int canAdd =
                        item.maxStack - slot.amount;

                    int addAmount =
                        Mathf.Min(canAdd, amount);

                    slot.amount += addAmount;

                    amount -= addAmount;

                    if (amount <= 0)
                    {
                        RefreshUI();
                        return true;
                    }
                }
            }
        }

        // EMPTY SLOT
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot.IsEmpty())
            {
                slot.item = item;

                if (item.stackable)
                {
                    int addAmount =
                        Mathf.Min(amount, item.maxStack);

                    slot.amount = addAmount;

                    amount -= addAmount;
                }
                else
                {
                    slot.amount = 1;
                    amount--;
                }

                if (amount <= 0)
                {
                    RefreshUI();
                    return true;
                }
            }
        }

        RefreshUI();

        return false;
    }

    // ================= REMOVE =================

    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
            return;

        InventorySlot slot = inventorySlots[slotIndex];

        if (slot.IsEmpty())
            return;

        slot.amount -= amount;

        if (slot.amount <= 0)
        {
            slot.Clear();
        }

        RefreshUI();
    }

    // ================= USE =================

    public void UseItem(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty())
            return;

        ItemData item = slot.item;

        switch (item.itemType)
        {
            case ItemType.Consumable:

                UseConsumable(slot);

                break;
        }
    }

    void UseConsumable(InventorySlot slot)
    {
        ItemData item = slot.item;

        // HEAL
        if (item.healAmount > 0)
        {
            if (playerStats.currentHealth <
                playerStats.maxHealth)
            {
                playerStats.Heal(item.healAmount);
            }
        }

        // INFECTION
        if (item.infectionReduce > 0)
        {
            if (playerStats.currentInfection > 0)
            {
                playerStats.ReduceInfection(
                    item.infectionReduce
                );
            }
        }

        slot.amount--;

        if (slot.amount <= 0)
        {
            slot.Clear();
        }

        if (useSound != null)
            audioSource.PlayOneShot(useSound);

        RefreshUI();
    }

    // ================= HOTBAR =================

    public void AssignToHotbar(int hotbarIndex, InventorySlot sourceSlot)
    {
        if (hotbarIndex < 0 ||
            hotbarIndex >= hotbarSlots.Length)
            return;

        hotbarSlots[hotbarIndex].item = sourceSlot.item;
        hotbarSlots[hotbarIndex].amount = sourceSlot.amount;

        RefreshUI();
    }

    void UseHotbarSlot(int index)
{
    InventorySlot slot =
        hotbarSlots[index];

    if (slot == null || slot.IsEmpty())
        return;

    ItemData item = slot.item;

    // CONSUMABLE
    if (item.itemType ==
        ItemType.Consumable)
    {
        PlayerStats player =
            FindFirstObjectByType<PlayerStats>();

        if (item.healAmount > 0)
        {
            player.Heal(item.healAmount);
        }

        if (item.infectionReduce > 0)
        {
            player.ReduceInfection(
                item.infectionReduce
            );
        }

        slot.amount--;

        if (slot.amount <= 0)
        {
            slot.Clear();
        }

        RefreshUI();
    }

    // WEAPON
    if (item.itemType ==
        ItemType.Weapon)
    {
        equippedWeapon =
            new InventorySlot(slot);

        RefreshUI();

        Debug.Log(
            "EQUIPPED: " +
            item.itemName
        );
    }
}
   
    public void UseHotbar(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length)
            return;

        InventorySlot slot = hotbarSlots[index];

        if (slot.IsEmpty())
            return;

        UseItem(slot);

        RefreshUI();
    }

    // ================= WEAPON =================

    public void EquipWeapon(InventorySlot slot)
    {
        if (slot == null ||
            slot.IsEmpty())
            return;

        if (slot.item.itemType != ItemType.Weapon)
            return;

        equippedWeapon.item = slot.item;
        equippedWeapon.amount = 1;

        RefreshUI();
    }

    // ================= MOVE =================

    public void MoveSlot(int from, int to)
    {
        if (from == to)
            return;

        InventorySlot temp = inventorySlots[from];

        inventorySlots[from] = inventorySlots[to];

        inventorySlots[to] = temp;

        RefreshUI();
    }

    // ================= UI =================

    public void RefreshUI()
    {
        InventoryUI ui =
            FindFirstObjectByType<InventoryUI>();

        if (ui != null)
        {
            ui.RefreshInventory();
        }
    }

    public void UpdateEquippedWeapon()
    {
        WeaponController weaponController =
            FindFirstObjectByType<WeaponController>();

        if (weaponController == null)
            return;

        // zawsze czyœcimy star¹ broñ
        weaponController.EquipWeapon(null);

        if (equippedWeapon == null ||
            equippedWeapon.IsEmpty() ||
            equippedWeapon.item == null)
        {
            return;
        }

        if (equippedWeapon.item.weaponData == null)
        {
            Debug.LogWarning(
                "Weapon item has no WeaponData assigned!"
            );
            return;
        }

        weaponController.EquipWeapon(
            equippedWeapon.item.weaponData
        );
    }
}