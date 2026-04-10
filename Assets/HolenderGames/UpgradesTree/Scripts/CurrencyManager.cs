using System;
using UnityEngine;

namespace HolenderGames.UpgradesTree
{
    // Helper class to handle a mockup game currency to be used in buying tree upgrades.
    // The class basically handles currency changes and invokes events to other systems to update their UI accordingly.
    public class CurrencyManager : MonoBehaviour
    {
        public static event Action<int> CurrencyChanged;
        public static CurrencyManager Instance { get; private set; }
        [SerializeField] private int currency;
        public int Currency { get { return currency; } }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        private void Start()
        {
            CurrencyChanged?.Invoke(currency);
        }

        private void OnEnable()
        {
            CurrencyChanged?.Invoke(currency);
        }

        public void ChangeCurrency(int amount)
        {
            currency += amount;
            CurrencyChanged?.Invoke(currency);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                ChangeCurrency(5);
                Debug.Log("Added 5 skill points");
            }
        }


    }
}