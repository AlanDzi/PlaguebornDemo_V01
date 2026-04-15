using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    [Header("Base Stats")]
    public int baseDamage = 20;
    public float attackSpeed = 1f;
    public float attackRange = 3f;

    [Header("Crit")]
    [Range(0f, 1f)]
    public float critChance = 0.1f;
    public float critMultiplier = 2f;
}