using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [System.Serializable]
    public class Loot
    {
        public ItemData item;
        [Range(0, 100)]
        public float dropChance = 100f;
        public int minAmount = 1;
        public int maxAmount = 1;
    }

    [Header("Loot Table")]
    public Loot[] loot;

    public void DropLoot()
    {
        if (InventoryManager.Instance == null)
            return;

        foreach (Loot l in loot)
        {
            if (l.item == null)
                continue;

            if (Random.Range(0f, 100f) <= l.dropChance)
            {
                int amount = Random.Range(l.minAmount, l.maxAmount + 1);

                InventoryManager.Instance.AddItem(l.item, amount);

                Debug.Log($"Dropped: {l.item.itemName} x{amount}");
            }
        }
    }
}