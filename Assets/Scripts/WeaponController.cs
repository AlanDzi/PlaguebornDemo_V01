using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float attackRange = 3f;
    public float attackCooldown = 0.8f;
    public float attackStaminaCost = 10f;

    [Header("Sword Transform")]
    public Transform swordTransform;

    [Header("Swing Animation")]
    public Vector3 swingRotation = new Vector3(-70f, 20f, 0f);
    public Vector3 swingPositionOffset = new Vector3(-0.15f, -0.1f, 0.25f);

    public float swingSpeed = 14f;
    public float returnSpeed = 16f;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioClip swingSound;

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

        if (swordTransform != null)
        {
            startRot = swordTransform.localRotation;
            startPos = swordTransform.localPosition;
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

    // ================= ATTACK =================

    bool CanAttack()
    {
        if (playerController == null) return false;

        return
            Time.time >= lastAttackTime + attackCooldown &&
            playerController.stamina >= attackStaminaCost &&
            !isSwinging;
    }

    void Attack()
    {
        if (playerController == null) return;

        lastAttackTime = Time.time;

        playerController.stamina -= attackStaminaCost;

        if (swingSound != null)
            audioSource.PlayOneShot(swingSound);

        StartCoroutine(SwingAnimation());

        DoRaycastDamage();
    }

    // ================= ANIMATION =================

    IEnumerator SwingAnimation()
    {
        if (swordTransform == null)
            yield break;

        isSwinging = true;

        Quaternion targetRot =
            startRot * Quaternion.Euler(swingRotation);

        Vector3 targetPos =
            startPos + swingPositionOffset;

        // ===== ATAK =====
        while (
            Quaternion.Angle(swordTransform.localRotation, targetRot) > 1f ||
            Vector3.Distance(swordTransform.localPosition, targetPos) > 0.01f
        )
        {
            swordTransform.localRotation = Quaternion.Slerp(
                swordTransform.localRotation,
                targetRot,
                Time.deltaTime * swingSpeed
            );

            swordTransform.localPosition = Vector3.Lerp(
                swordTransform.localPosition,
                targetPos,
                Time.deltaTime * swingSpeed
            );

            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        // ===== POWRÓT =====
        while (
            Quaternion.Angle(swordTransform.localRotation, startRot) > 1f ||
            Vector3.Distance(swordTransform.localPosition, startPos) > 0.01f
        )
        {
            swordTransform.localRotation = Quaternion.Slerp(
                swordTransform.localRotation,
                startRot,
                Time.deltaTime * returnSpeed
            );

            swordTransform.localPosition = Vector3.Lerp(
                swordTransform.localPosition,
                startPos,
                Time.deltaTime * returnSpeed
            );

            yield return null;
        }

        swordTransform.localRotation = startRot;
        swordTransform.localPosition = startPos;

        isSwinging = false;
    }

    // ================= DAMAGE =================

    void DoRaycastDamage()
    {
        RaycastHit hit;

        Vector3 origin = playerCamera.transform.position;
        Vector3 dir = playerCamera.transform.forward;

        Debug.DrawRay(origin, dir * attackRange, Color.red, 0.5f);

        if (Physics.Raycast(origin, dir, out hit, attackRange))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            if (enemy != null)
            {
                int dmg = playerStats.currentDamage;
                enemy.TakeDamage(dmg);

                if (hitSound != null)
                    audioSource.PlayOneShot(hitSound);
            }
            else
            {
                if (missSound != null)
                    audioSource.PlayOneShot(missSound);
            }
        }
    }
}
