using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace HolenderGames.UpgradesTree
{
    // A simple class to view the current amount of currency the player have.
    // with a simple animation whenever the currency changes.
    public class CurrencyViewer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtCurrency;
        private void OnEnable()
        {
            CurrencyManager.CurrencyChanged += OnCurrencyChange;
        }

        private void OnDisable()
        {
            CurrencyManager.CurrencyChanged -= OnCurrencyChange;
        }

        private void OnCurrencyChange(int currentCurrency)
        {
            txtCurrency.text = currentCurrency.ToString();

            //animation
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOShakeScale(0.2f,strength:0.2f);
        }
    }
}