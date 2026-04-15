using System.Runtime.CompilerServices;
using UnityEngine;

namespace HolenderGames.UpgradesTree
{
    // The manager class to handle the Tree of upgrades.
    // The main purpose of the class is the handle the purchasing of upgrades and unlocking of child upgrades.
    // Use this class as the main point of handling the logic of your game when and upgrade is bought.
    public class UpgradesTreeManager : MonoBehaviour
    {
        [SerializeField] CurrencyManager currencyManager;
        private UpgradeView[] upgrades;
        void Awake()
        {
            upgrades = GetComponentsInChildren<UpgradeView>();
        }

        private void Start()
        {
            foreach (UpgradeView upgrade in upgrades)
            {
                upgrade.OnBuyUpgrade += OnBuyUpgrade;
                upgrade.Data.ResetUpgrade();
                upgrade.UpdateUI();
            }
        }

        private void OnBuyUpgrade(UpgradeData upgrade)
        {
            int cost = -upgrade.Cost; // caching the cost for this level before upgrading. 
            upgrade.LevelUp();
            currencyManager.ChangeCurrency(cost); // applying cost after upgrading to refresh the tree UI
            //check if any children upgrades should be unlocked
            UnlockUpgrades(upgrade);
        }

        private void UnlockUpgrades(UpgradeData parentUpgrade)
        {
            bool isMaxed = parentUpgrade.IsMaxLevel;
            int current = parentUpgrade.CurrentLevel;
            foreach (UpgradeView upgrade in upgrades)
            {
                var requiredParent = upgrade.Data.ParentUpgrade;

                if (requiredParent != null && requiredParent == parentUpgrade)
                {
                    if (upgrade.Data.UnlockWhenParentMaxed && isMaxed)
                    {
                        upgrade.Unlock(currencyManager.Currency);
                    }
                    else if (!upgrade.Data.UnlockWhenParentMaxed && upgrade.Data.UnlockAtParentLevel <= current)
                    {
                        upgrade.Unlock(currencyManager.Currency);
                    }
                }
            }
        }

    }
}