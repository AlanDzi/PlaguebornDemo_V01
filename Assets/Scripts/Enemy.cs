using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Melee,
    Ranger
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.Melee;

    [Header("Enemy Stats")]
    public int maxHealth = 50;
    public int currentHealth;
    public int experienceReward = 25;

    [Header("Movement i AI")]
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float detectionRange = 8f;
    public float attackCooldown = 2f;

    [Header("Combat")]
    public int attackDamage = 10;

    [Header("Ranger Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileForce = 15f;
    public float shootingRange = 12f;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip deathSound;

    public AudioClip detectMeleeSound;
    public AudioClip detectRangerSound;

    private AudioSource audioSource;
    private Transform player;
    private PlayerStats playerStats;
    private NavMeshAgent navAgent;
    private GameManager gameManager;

    private bool isDead = false;
    private bool playerDetected = false;
    private bool detectionSoundPlayed = false;

    private float lastAttackTime = 0f;

    void Start()
    {
        currentHealth = maxHealth;

        PlayerController pc = FindFirstObjectByType<PlayerController>();

        if (pc != null)
        {
            player = pc.transform;
            playerStats = pc.GetComponent<PlayerStats>();
        }

        gameManager = FindFirstObjectByType<GameManager>();

        navAgent = GetComponent<NavMeshAgent>();

        if (navAgent != null)
        {
            navAgent.speed = moveSpeed;
            navAgent.stoppingDistance = attackRange - 0.2f;
            navAgent.acceleration = 12f;
            navAgent.angularSpeed = 360f;
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        if (isDead || player == null || playerStats == null)
            return;

        if (playerStats.currentHealth <= 0)
        {
            if (navAgent != null && navAgent.enabled)
                navAgent.ResetPath();

            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // Reset po zgubieniu gracza (opcjonalnie)
        if (distance > detectionRange * 2f)
        {
            playerDetected = false;
            detectionSoundPlayed = false;
        }

        // Wykrycie gracza
        if (!playerDetected && distance <= detectionRange)
        {
            playerDetected = true;
            PlayDetectionSound();
        }

        if (!playerDetected)
            return;

        if (enemyType == EnemyType.Melee)
            HandleMelee(distance);
        else
            HandleRanger(distance);
    }

    // ================= MELEE =================

    void HandleMelee(float distance)
    {
        if (distance <= attackRange)
        {
            if (navAgent != null && navAgent.enabled)
                navAgent.ResetPath();

            FacePlayer();

            if (Time.time >= lastAttackTime + attackCooldown)
                AttackPlayer();
        }
        else if (distance <= detectionRange * 1.5f)
        {
            MoveTowardsPlayer();
        }
    }

    void AttackPlayer()
    {
        lastAttackTime = Time.time;

        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        playerStats.TakeDamage(attackDamage);

        int infection = Random.Range(10, 25);
        playerStats.AddInfection(infection);

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 dir =
                (player.position - transform.position).normalized;

            rb.AddForce(dir * 5f, ForceMode.Impulse);
        }
    }

    // ================= RANGER =================

    void HandleRanger(float distance)
    {
        if (distance <= shootingRange)
        {
            if (navAgent != null && navAgent.enabled)
                navAgent.ResetPath();

            FacePlayer();

            if (Time.time >= lastAttackTime + attackCooldown)
                Shoot();
        }
        else if (distance <= detectionRange * 1.5f)
        {
            MoveTowardsPlayer();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || shootPoint == null)
            return;

        lastAttackTime = Time.time;

        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        GameObject proj = Instantiate(
            projectilePrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(
                shootPoint.forward * projectileForce,
                ForceMode.Impulse
            );
        }
    }

    // ================= COMMON =================

    void MoveTowardsPlayer()
    {
        if (navAgent != null && navAgent.enabled)
            navAgent.SetDestination(player.position);
    }

    void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void PlayDetectionSound()
    {
        if (detectionSoundPlayed || audioSource == null)
            return;

        AudioClip clip = null;

        if (enemyType == EnemyType.Melee)
            clip = detectMeleeSound;
        else if (enemyType == EnemyType.Ranger)
            clip = detectRangerSound;

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            detectionSoundPlayed = true;
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead)
            return;

        currentHealth -= dmg;

        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
            Die();
        else
            playerDetected = true;
    }

    System.Collections.IEnumerator DamageFlash()
    {
        Renderer r = GetComponent<Renderer>();

        if (r != null)
        {
            Color col = r.material.color;

            r.material.color = Color.red;

            yield return new WaitForSeconds(0.15f);

            if (!isDead)
                r.material.color = col;
        }
    }

    void Die()
    {
        isDead = true;

        if (navAgent != null)
            navAgent.enabled = false;

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        playerStats.AddExperience(experienceReward);

        InventoryManager inv =
            FindFirstObjectByType<InventoryManager>();

        if (inv != null)
            inv.AddCoins(Random.Range(5, 16));

        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        if (gameManager != null)
            StartCoroutine(CheckVictory());

        StartCoroutine(DeathAnim());
    }

    System.Collections.IEnumerator CheckVictory()
    {
        yield return new WaitForSeconds(0.5f);
        gameManager.CheckForVictory();
    }

    System.Collections.IEnumerator DeathAnim()
    {
        float t = 0f;
        float d = 1.2f;

        Vector3 rot = transform.eulerAngles;
        Vector3 pos = transform.position;

        float fall = Random.Range(0, 2) == 0 ? 90 : -90;

        while (t < d)
        {
            t += Time.deltaTime;

            float p = Mathf.SmoothStep(0, 1, t / d);

            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(rot),
                Quaternion.Euler(new Vector3(fall, rot.y, rot.z)),
                p
            );

            transform.position = Vector3.Lerp(
                pos,
                pos + Vector3.down * 0.3f,
                p
            );

            yield return null;
        }

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}