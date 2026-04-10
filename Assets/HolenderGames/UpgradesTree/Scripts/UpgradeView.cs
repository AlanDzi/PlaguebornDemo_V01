using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HolenderGames.UpgradesTree
{
    // A class to handle the view of each upgrade.
    // It hold the scriptable object which contains the data of the upgrade and updates the UI accordingly.
    // This class also handles clicking (buying) the upgrade, updating the view when currency changes and update becomes affordable or not.
    public class UpgradeView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<UpgradeData> OnBuyUpgrade;

        [SerializeField] private UpgradeData upgradeData;
        [SerializeField] private Image bg;
        [SerializeField] private Image icon;
        private Button btnUpgrade;
        public UpgradeTooltip tooltip; 
        public UpgradeData Data {  get { return upgradeData; } }


        private void Awake()
        {
            btnUpgrade = GetComponent<Button>();
            btnUpgrade.onClick.AddListener(OnUpgradeClick);
        }

        void Start()
        {
            if(upgradeData == null)
            {
                Debug.LogError("Assign an UpgradeData scriptable object to prefab " + name);
                return;
            }
        }

        private void OnEnable()
        {
            CurrencyManager.CurrencyChanged += OnCurrencyChange;

            if(CurrencyManager.Instance !=null)
                OnCurrencyChange(CurrencyManager.Instance.Currency);
        }

        private void OnDisable()
        {
            CurrencyManager.CurrencyChanged -= OnCurrencyChange;
        }

        private void OnCurrencyChange(int currentCurrency)
        {
            if (upgradeData == null)
            {
                return;
            }
            if (upgradeData.IsMaxLevel)
                return;

            bg.color = currentCurrency < upgradeData.Cost ? Color.red : Color.green;
            btnUpgrade.interactable = currentCurrency >= upgradeData.Cost;
        }

        public void OnUpgradeClick()
        {
            if (upgradeData == null)
            {
                Debug.LogError("Assign an UpgradeData scriptable object to prefab " + name);
                return;
            }
            OnBuyUpgrade?.Invoke(upgradeData);


            if (upgradeData.IsMaxLevel)
            {
                btnUpgrade.interactable = false;
                bg.color = Color.yellow;
            }


            tooltip.UpdateTooltip(upgradeData.Description, upgradeData.CurrentLevel, upgradeData.Costs.Length, upgradeData.Cost);

            //animation
            bg.transform.DOKill();
            bg.transform.localScale = Vector3.one;
            bg.transform.DOShakeScale(0.3f, strength: 0.2f);

        }

        public void Unlock(int currentCurrency)
        {
            gameObject.SetActive(true);
            OnCurrencyChange(currentCurrency);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip.gameObject.SetActive(false);
        }

        public void UpdateUI()
        {
            tooltip.InitTooltip(upgradeData.UpgradeName, upgradeData.Icon);
            tooltip.UpdateTooltip(upgradeData.Description, upgradeData.CurrentLevel, upgradeData.Costs.Length, upgradeData.Cost);
            icon.sprite = upgradeData.Icon;

            // if has requirement upgrade then start hidden
            if (upgradeData.ParentUpgrade != null)
            {
                gameObject.SetActive(false);
            }
        }


    }
}