using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Items")]
    public int antidotes = 0;
    public int bandages = 0;
    public int maxItems = 3;

    [Header("Currency")]
    public int coins = 0;
    public int maxCoins = 100;

    [Header("Item Effects")]
    public int bandageHealAmount = 20;

    [Header("Cooldown")]
    public float itemCooldown = 5f;

    private float lastAntidoteUse = -999f;
    private float lastBandageUse = -999f;

    [Header("Audio")]
    public AudioClip antidoteUseSound;
    public AudioClip bandageUseSound;

    private PlayerStats playerStats;
    private InventoryUI inventoryUI;
    private AudioSource audioSource;

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        UpdateUI();
    }

    void Update()
    {
        // BLOKUJEMY PODCZAS UI
        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && CanUseBandage())
            UseBandage();

        if (Input.GetKeyDown(KeyCode.Alpha2) && CanUseAntidote())
            UseAntidote();
    }

    // ================= CHECK =================

    public bool CanUseAntidote()
    {
        return antidotes > 0 &&
               playerStats != null &&
               playerStats.currentInfection > 0 &&
               Time.time >= lastAntidoteUse + itemCooldown;
    }

    public bool CanUseBandage()
    {
        return bandages > 0 &&
               playerStats != null &&
               playerStats.currentHealth < playerStats.maxHealth &&
               Time.time >= lastBandageUse + itemCooldown;
    }

    // ================= USE =================

    public void UseAntidote()
    {
        if (!CanUseAntidote()) return;

        antidotes--;
        lastAntidoteUse = Time.time;

        playerStats.ReduceInfection(playerStats.currentInfection);

        if (antidoteUseSound != null)
            audioSource.PlayOneShot(antidoteUseSound);

        UpdateUI();
    }

    public void UseBandage()
    {
        if (!CanUseBandage()) return;

        bandages--;
        lastBandageUse = Time.time;

        playerStats.Heal(bandageHealAmount);

        if (bandageUseSound != null)
            audioSource.PlayOneShot(bandageUseSound);

        UpdateUI();
    }

    // ================= ADD =================

    public void AddCoins(int amount)
    {
        coins = Mathf.Clamp(coins + amount, 0, maxCoins);
        UpdateUI();
    }

    public bool AddBandage(int amount)
    {
        if (bandages >= maxItems) return false;

        bandages = Mathf.Clamp(bandages + amount, 0, maxItems);
        UpdateUI();
        return true;
    }

    public bool AddAntidote(int amount)
    {
        if (antidotes >= maxItems) return false;

        antidotes = Mathf.Clamp(antidotes + amount, 0, maxItems);
        UpdateUI();
        return true;
    }

    // ================= SHOP =================

    public bool CanBuyItem(int cost)
    {
        return coins >= cost;
    }

    public bool CanBuyAntidote(int cost)
    {
        return CanBuyItem(cost) && antidotes < maxItems;
    }

    public bool CanBuyBandage(int cost)
    {
        return CanBuyItem(cost) && bandages < maxItems;
    }

    public bool BuyAntidote(int cost)
    {
        if (!CanBuyAntidote(cost)) return false;

        coins -= cost;
        antidotes++;
        UpdateUI();
        return true;
    }

    public bool BuyBandage(int cost)
    {
        if (!CanBuyBandage(cost)) return false;

        coins -= cost;
        bandages++;
        UpdateUI();
        return true;
    }

    // ================= UI =================

    private void UpdateUI()
    {
        if (inventoryUI != null)
            inventoryUI.UpdateInventoryDisplay();
    }
}