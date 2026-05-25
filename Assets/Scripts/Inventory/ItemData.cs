using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic")]
    public string itemName;
    [TextArea] public string description;

    public Sprite icon;

    public ItemType itemType;

    [Header("Stack")]
    public bool stackable = false;
    public int maxStack = 1;

    [Header("World")]
    public GameObject worldPrefab;

    [Header("Consumable")]
    public int healAmount;
    public int infectionReduce;

    [Header("Weapon")]
    public WeaponData weaponData;
}