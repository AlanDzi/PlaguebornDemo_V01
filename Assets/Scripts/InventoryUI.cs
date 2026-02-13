using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Item UI - Bottom")]
    public Image antidoteIcon;
    public TextMeshProUGUI antidoteCount;
    public TextMeshProUGUI antidoteKey;

    public Image bandageIcon;
    public TextMeshProUGUI bandageCount;
    public TextMeshProUGUI bandageKey;

    [Header("Currency UI - Top Right")]
    public Image shopIcon;
    public TextMeshProUGUI shopKey;
    public Image coinIcon;
    public TextMeshProUGUI coinCount;

    [Header("Shop Panel (ShopCanvas)")]
    public Canvas shopCanvas;
    public GameObject shopPanel;
    public Button shopOpenButton;
    public Button shopCloseButton;

    [Header("Shop Items")]
    public Button buyAntidoteButton;
    public TextMeshProUGUI antidotePrice;
    public Button buyBandageButton;
    public TextMeshProUGUI bandagePrice;

    [Header("Prices")]
    public int antidoteCost = 60;
    public int bandageCost = 25;

    [Header("Audio")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;

    private AudioSource audioSource;
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (shopCanvas != null)
            shopCanvas.gameObject.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (antidotePrice != null)
            antidotePrice.text = antidoteCost + "$";

        if (bandagePrice != null)
            bandagePrice.text = bandageCost + "$";

        if (antidoteKey != null)
            antidoteKey.text = "2";

        if (bandageKey != null)
            bandageKey.text = "1";

        if (shopKey != null)
            shopKey.text = "T";

        SetupButtons();
        UpdateInventoryDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleShop();
        }

        UpdateShopButtons();
    }

    // ================= BUTTON SETUP =================

    void SetupButtons()
    {
        if (shopOpenButton != null)
        {
            shopOpenButton.onClick.AddListener(OpenShop);
            AddButtonEffects(shopOpenButton);
        }

        if (shopCloseButton != null)
        {
            shopCloseButton.onClick.AddListener(CloseShop);
            AddButtonEffects(shopCloseButton);
        }

        if (buyAntidoteButton != null)
        {
            buyAntidoteButton.onClick.AddListener(BuyAntidote);
            AddButtonEffects(buyAntidoteButton);
        }

        if (buyBandageButton != null)
        {
            buyBandageButton.onClick.AddListener(BuyBandage);
            AddButtonEffects(buyBandageButton);
        }
    }

    void AddButtonEffects(Button button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();

        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((_) => OnButtonHover(button));

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((_) => OnButtonExit(button));

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);

        button.onClick.AddListener(PlayButtonClickSound);
    }

    // ================= BUTTON FX =================

    void OnButtonHover(Button button)
    {
        if (buttonHoverSound != null)
            audioSource.PlayOneShot(buttonHoverSound, 0.5f);

        Image img = button.GetComponent<Image>();
        if (img != null)
            img.color *= 0.8f;
    }

    void OnButtonExit(Button button)
    {
        Image img = button.GetComponent<Image>();
        if (img != null)
            img.color = Color.white;
    }

    void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }

    // ================= UI UPDATE =================

    public void UpdateInventoryDisplay()
    {
        if (inventoryManager == null) return;

        if (antidoteCount != null)
            antidoteCount.text = inventoryManager.antidotes + "/" + inventoryManager.maxItems;

        if (bandageCount != null)
            bandageCount.text = inventoryManager.bandages + "/" + inventoryManager.maxItems;

        if (coinCount != null)
            coinCount.text = inventoryManager.coins + "/" + inventoryManager.maxCoins;
    }

    void UpdateShopButtons()
    {
        if (inventoryManager == null) return;

        if (buyAntidoteButton != null)
            buyAntidoteButton.interactable =
                inventoryManager.CanBuyAntidote(antidoteCost);

        if (buyBandageButton != null)
            buyBandageButton.interactable =
                inventoryManager.CanBuyBandage(bandageCost);
    }

    // ================= SHOP =================

    public void OpenShop()
    {
        if (shopCanvas != null)
            shopCanvas.gameObject.SetActive(true);

        if (shopPanel != null)
            shopPanel.SetActive(true);

        Time.timeScale = 0f;

        DisablePlayer();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseShop()
    {
        if (shopCanvas != null)
            shopCanvas.gameObject.SetActive(false);

        if (shopPanel != null)
            shopPanel.SetActive(false);

        Time.timeScale = 1f;

        EnablePlayer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ToggleShop()
    {
        if (shopPanel == null) return;

        if (shopPanel.activeSelf)
            CloseShop();
        else
            OpenShop();
    }

    // ================= BUY =================

    public void BuyAntidote()
    {
        if (inventoryManager != null)
            inventoryManager.BuyAntidote(antidoteCost);
    }

    public void BuyBandage()
    {
        if (inventoryManager != null)
            inventoryManager.BuyBandage(bandageCost);
    }

    // ================= PLAYER =================

    void DisablePlayer()
    {
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        WeaponController wc = FindFirstObjectByType<WeaponController>();

        if (pc != null) pc.enabled = false;
        if (wc != null) wc.enabled = false;
    }

    void EnablePlayer()
    {
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        WeaponController wc = FindFirstObjectByType<WeaponController>();

        if (pc != null) pc.enabled = true;
        if (wc != null) wc.enabled = true;
    }
}
