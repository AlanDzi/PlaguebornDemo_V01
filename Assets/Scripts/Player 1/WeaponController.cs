using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Weapons")]
    public WeaponData[] weapons;
    int currentWeaponIndex = 0;


[Header("Current Weapon")]
    public WeaponData weaponData;

    [Header("Weapon Holder")]
    public Transform weaponHolder;
    private Transform currentWeapon;

    [Header("Sway")]
    public float swayAmount = 5f;
    public float swaySmooth = 6f;

    [Header("Clipping")]
    public float clipStartDistance = 0.5f;
    public float minDistance = 0.2f;
    public float maxDistance = 0.6f;
    public float clipSmooth = 10f;
    public LayerMask clipMask;

    PlayerStats playerStats;
    PlayerController playerController;
    Camera playerCamera;

    float lastAttackTime;
    AudioSource audioSource;

    Quaternion startRot;
    Vector3 startPos;

    bool isSwinging = false;

    Vector3 swayOffset;
    float currentDistance;

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

       

        currentDistance = maxDistance;
    }

    void Update()
    {
        if (UIManager.Instance != null &&
            UIManager.Instance.IsAnyUIOpen)
            return;

        if (Input.GetMouseButtonDown(0) &&
            CanAttack())
        {
            Attack();
        }

        HandleSway();
        HandleClipping();
        ApplyIdleTransform();
    }



    public void EquipWeapon(WeaponData newWeapon)
    {
        weaponData = newWeapon;

        if (weaponData == null)
        {
            if (currentWeapon != null)
                Destroy(currentWeapon.gameObject);

            currentWeapon = null;
            return;
        }

        SpawnWeapon();

        Debug.Log(
            "Equipped: " +
            weaponData.weaponName
        );
    }

    // ================= SPAWN =================

    void SpawnWeapon()
    {
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        GameObject obj = Instantiate(weaponData.weaponPrefab, weaponHolder);
        currentWeapon = obj.transform;

        startRot = currentWeapon.localRotation;
        startPos = currentWeapon.localPosition;
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
        if (weaponData == null) return;
        
        if (currentWeapon == null)
            SpawnWeapon();

        lastAttackTime = Time.time;

        playerController.stamina -= weaponData.staminaCost;

        if (weaponData.swingSound != null)
            audioSource.PlayOneShot(weaponData.swingSound);

        StartCoroutine(SwingAnimation());
        DoRaycastDamage();
    }

    // ================= SWAY =================

    void HandleSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float moveX = -mouseX * swayAmount;
        float moveY = -mouseY * swayAmount;

        swayOffset = Vector3.Lerp(
            swayOffset,
            new Vector3(moveX, moveY, 0f),
            Time.deltaTime * swaySmooth
        );
    }

    // ================= CLIPPING =================

    void HandleClipping()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, clipMask))
        {
            if (hit.distance < clipStartDistance)
            {
                float t = Mathf.InverseLerp(clipStartDistance, minDistance, hit.distance);
                float targetDist = Mathf.Lerp(maxDistance, minDistance, t);

                currentDistance = Mathf.Lerp(currentDistance, targetDist, Time.deltaTime * clipSmooth);
            }
            else
            {
                currentDistance = Mathf.Lerp(currentDistance, maxDistance, Time.deltaTime * clipSmooth);
            }
        }
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, maxDistance, Time.deltaTime * clipSmooth);
        }
    }

    // ================= IDLE =================

    void ApplyIdleTransform()
    {
        if (currentWeapon == null || isSwinging) return;

        Vector3 finalPos = startPos;

        // sway
        finalPos += swayOffset * 0.01f;

        // clipping offset
        float clipOffset = currentDistance - maxDistance;
        finalPos += Vector3.forward * clipOffset;

        currentWeapon.localPosition = Vector3.Lerp(
            currentWeapon.localPosition,
            finalPos,
            Time.deltaTime * 10f
        );
    }

    // ================= ANIMATION =================

    IEnumerator SwingAnimation()
    {
        if (weaponData == null)
            yield break;

        if (currentWeapon == null)
        {
            SpawnWeapon();

            if (currentWeapon == null)
                yield break;
        }

        isSwinging = true;

        Quaternion targetRot =
            startRot *
            Quaternion.Euler(
                weaponData.swingRotation
            );

        Vector3 targetPos =
            startPos +
            weaponData.swingPositionOffset;

        while (
            Quaternion.Angle(
                currentWeapon.localRotation,
                targetRot
            ) > 1f ||

            Vector3.Distance(
                currentWeapon.localPosition,
                targetPos
            ) > 0.01f
        )
        {
            if (currentWeapon == null)
                yield break;

            currentWeapon.localRotation =
                Quaternion.Slerp(
                    currentWeapon.localRotation,
                    targetRot,
                    Time.deltaTime *
                    weaponData.swingSpeed
                );

            currentWeapon.localPosition =
                Vector3.Lerp(
                    currentWeapon.localPosition,
                    targetPos,
                    Time.deltaTime *
                    weaponData.swingSpeed
                );

            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        while (
            Quaternion.Angle(
                currentWeapon.localRotation,
                startRot
            ) > 1f ||

            Vector3.Distance(
                currentWeapon.localPosition,
                startPos
            ) > 0.01f
        )
        {
            if (currentWeapon == null)
                yield break;

            currentWeapon.localRotation =
                Quaternion.Slerp(
                    currentWeapon.localRotation,
                    startRot,
                    Time.deltaTime *
                    weaponData.returnSpeed
                );

            currentWeapon.localPosition =
                Vector3.Lerp(
                    currentWeapon.localPosition,
                    startPos,
                    Time.deltaTime *
                    weaponData.returnSpeed
                );

            yield return null;
        }

        if (currentWeapon != null)
        {
            currentWeapon.localRotation =
                startRot;

            currentWeapon.localPosition =
                startPos;
        }

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
