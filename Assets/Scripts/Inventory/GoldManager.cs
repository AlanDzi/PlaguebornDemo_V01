using System;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    public static event Action<int> GoldChanged;

    [SerializeField]
    private int gold;

    public int Gold => gold;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        GoldChanged?.Invoke(gold);

        
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount)
            return false;

        gold -= amount;

        GoldChanged?.Invoke(gold);

        return true;
    }

    public void SetGold(int amount)
    {
        gold = amount;

        GoldChanged?.Invoke(gold);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            AddGold(100);

            Debug.Log(
                "Gold: " + gold
            );
        }
    }
}