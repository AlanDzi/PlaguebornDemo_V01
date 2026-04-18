using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    [Header("Base Stats")]
    public int baseDamage = 20;
    public float attackSpeed = 1f;
    public float attackRange = 3f;

    [Header("Attack Settings")]
    public float attackCooldown = 0.8f;
    public float staminaCost = 10f;

    [Header("Crit")]
    [Range(0f, 1f)]
    public float critChance = 0.1f;
    public float critMultiplier = 2f;

    [Header("Visual")]
    public GameObject weaponPrefab;

    [Header("Animation")]
    public Vector3 swingRotation = new Vector3(-70f, 20f, 0f);
    public Vector3 swingPositionOffset = new Vector3(-0.15f, -0.1f, 0.25f);
    public float swingSpeed = 14f;
    public float returnSpeed = 16f;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioClip swingSound;
}