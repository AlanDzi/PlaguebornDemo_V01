using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HolenderGames.UpgradesTree
{
    // A class that handles viewing of the tooltip above each upgrade.
    public class UpgradeTooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtCost;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private TextMeshProUGUI txtCurrentLevel;
        [SerializeField] private TextMeshProUGUI txtMaxLevel;
        [SerializeField] private Image icon;
        [SerializeField] private RectTransform headerLayoutRoot;

        private bool isFirstEnabled = true;
        private void OnEnable()
        {
            // Animation
            transform.DOKill();
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            transform.DOShakeRotation(0.3f,strength:20,vibrato:20);    
            transform.DOShakeScale(0.2f,strength:0.5f,vibrato:5);

            // fixing layout issues with content fitter
            if (isFirstEnabled)
            {
                isFirstEnabled = false;
                StartCoroutine(RefreshLayoutNextFrame());
            }
        }


        // first time initializing of the constant parameters of the class: name, icon.
        public void InitTooltip(string upgradeName, Sprite icon)
        {
            // Init the base values of the upgrade (doesn't change)
            txtName.text = upgradeName;
            this.icon.sprite = icon;
        }
        // Updating the tooltip whenever a new level of the upgrade is purchased.
        public void UpdateTooltip(string description, int currentLevel, int maxLevel, int cost)
        {
            // Init and update the variables of the upgrade (changes every level)
            txtCost.text = cost.ToString();
            txtDescription.text = description;
            txtCurrentLevel.text = currentLevel.ToString();
            txtMaxLevel.text = maxLevel.ToString();

            if (currentLevel >= maxLevel)
            {
                txtCost.text = "MAX";
            }
        }


        // Fix and issue where the header of the tooltip was not fast enough to update with text size.
        private IEnumerator RefreshLayoutNextFrame()
        {
            yield return null; // wait one frame for layout system to catch up
            txtName.ForceMeshUpdate(true);
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(headerLayoutRoot);
        }

    }
}