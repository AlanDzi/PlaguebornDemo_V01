using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData weaponData;

    [Header("Weapon Holder")]
    public Transform weaponHolder;

    private Transform currentWeapon;

    PlayerStats playerStats;
    PlayerController playerController;
    Camera playerCamera;

    float lastAttackTime;
    AudioSource audioSource;

    Quaternion startRot;
    Vector3 startPos;

    bool isSwinging = false;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        playerController = GetComponent<PlayerController>();

        playerCamera = Camera.main;

        if (playerCamera == null)
            playerCamera = FindFirstObjectByType<Camera>();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (weaponData != null && weaponData.weaponPrefab != null)
        {
            SpawnWeapon();
        }
    }

    void Update()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
            return;

        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            Attack();
        }
    }

    // ================= SPAWN WEAPON =================

    void SpawnWeapon()
    {
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        GameObject obj = Instantiate(weaponData.weaponPrefab, weaponHolder);
        currentWeapon = obj.transform;

        startRot = currentWeapon.localRotation;
        startPos = currentWeapon.localPosition;
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        weaponData = newWeapon;
        SpawnWeapon();
    }

    // ================= ATTACK =================

    bool CanAttack()
    {
        if (playerController == null || weaponData == null) return false;

        return
            Time.time >= lastAttackTime + (weaponData.attackCooldown / weaponData.attackSpeed) &&
            playerController.stamina >= weaponData.staminaCost &&
            !isSwinging;
    }

    void Attack()
    {
        if (playerController == null || weaponData == null)
            return;

        lastAttackTime = Time.time;

        playerController.stamina -= weaponData.staminaCost;

        if (weaponData.swingSound != null)
            audioSource.PlayOneShot(weaponData.swingSound);

        StartCoroutine(SwingAnimation());

        DoRaycastDamage();
    }

    // ================= ANIMATION =================

    IEnumerator SwingAnimation()
    {
        if (currentWeapon == null || weaponData == null)
            yield break;

        isSwinging = true;

        Quaternion targetRot =
            startRot * Quaternion.Euler(weaponData.swingRotation);

        Vector3 targetPos =
            startPos + weaponData.swingPositionOffset;

        // ATAK
        while (
            Quaternion.Angle(currentWeapon.localRotation, targetRot) > 1f ||
            Vector3.Distance(currentWeapon.localPosition, targetPos) > 0.01f
        )
        {
            currentWeapon.localRotation = Quaternion.Slerp(
                currentWeapon.localRotation,
                targetRot,
                Time.deltaTime * weaponData.swingSpeed
            );

            currentWeapon.localPosition = Vector3.Lerp(
                currentWeapon.localPosition,
                targetPos,
                Time.deltaTime * weaponData.swingSpeed
            );

            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        // POWRÓT
        while (
            Quaternion.Angle(currentWeapon.localRotation, startRot) > 1f ||
            Vector3.Distance(currentWeapon.localPosition, startPos) > 0.01f
        )
        {
            currentWeapon.localRotation = Quaternion.Slerp(
                currentWeapon.localRotation,
                startRot,
                Time.deltaTime * weaponData.returnSpeed
            );

            currentWeapon.localPosition = Vector3.Lerp(
                currentWeapon.localPosition,
                startPos,
                Time.deltaTime * weaponData.returnSpeed
            );

            yield return null;
        }

        currentWeapon.localRotation = startRot;
        currentWeapon.localPosition = startPos;

        isSwinging = false;
    }

    // ================= DAMAGE =================

    void DoRaycastDamage()
    {
        if (weaponData == null) return;

        RaycastHit hit;

        Vector3 origin = playerCamera.transform.position;
        Vector3 dir = playerCamera.transform.forward;

        float totalRange = weaponData.attackRange + playerStats.attackRange;

        Debug.DrawRay(origin, dir * totalRange, Color.red, 0.5f);

        if (Physics.Raycast(origin, dir, out hit, totalRange))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            if (enemy != null)
            {
                int dmg = weaponData.baseDamage + playerStats.currentDamage;

                float totalCritChance = weaponData.critChance + playerStats.critChance;
                float totalCritMultiplier = weaponData.critMultiplier * playerStats.critMultiplier;

                if (Random.value < totalCritChance)
                {
                    dmg = Mathf.RoundToInt(dmg * totalCritMultiplier);
                    Debug.Log("CRIT!");
                }

                enemy.TakeDamage(dmg);

                if (weaponData.hitSound != null)
                    audioSource.PlayOneShot(weaponData.hitSound);
            }
            else
            {
                if (weaponData.missSound != null)
                    audioSource.PlayOneShot(weaponData.missSound);
            }
        }
    }
}