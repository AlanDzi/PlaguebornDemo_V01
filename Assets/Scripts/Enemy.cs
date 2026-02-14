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

    [Header("Movement & AI")]
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float detectionRange = 8f;
    public float attackCooldown = 2f;

    [Header("Combat")]
    public int attackDamage = 10;

    [Header("Infection / Poison")]
    public int infectionMin = 0;
    public int infectionMax = 0;

    [Header("Ranger")]
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
    private Animator animator;

    private Transform player;
    private PlayerStats playerStats;
    private NavMeshAgent navAgent;
    private GameManager gameManager;

    private bool isDead = false;
    private bool playerDetected = false;
    private bool detectionSoundPlayed = false;

    private bool isAttacking = false;
    private bool damageDealt = false;

    private float lastAttackTime = 0f;



    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

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

        if (distance > detectionRange * 2f)
        {
            playerDetected = false;
            detectionSoundPlayed = false;
        }

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

   

    void HandleMelee(float distance)
    {
        if (isAttacking) return;

        if (distance <= attackRange)
        {
            StopMoving();
            FacePlayer();

            if (Time.time >= lastAttackTime + attackCooldown)
                StartAttack();
        }
        else if (distance <= detectionRange * 1.5f)
        {
            MoveTowardsPlayer();
        }
    }

    void StartAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        damageDealt = false;
        lastAttackTime = Time.time;

        StopMoving();

        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);

            int rand = Random.Range(0, 2);

            if (rand == 0)
                animator.Play("Gnom_Attack", 0, 0f);
            else
                animator.Play("Gnom_slash", 0, 0f);
        }
    }

   

    public void DealDamage()
    {
        if (isDead || player == null) return;

        if (damageDealt) return;

        damageDealt = true;

        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);

        playerStats.TakeDamage(attackDamage);

        
        if (infectionMax > 0)
        {
            int infection = Random.Range(infectionMin, infectionMax + 1);
            playerStats.AddInfection(infection);
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 dir =
                (player.position - transform.position).normalized;

            rb.AddForce(dir * 5f, ForceMode.Impulse);
        }
    }


    public void EndAttack()
    {
        isAttacking = false;
        damageDealt = false;

        if (animator != null)
            animator.SetBool("IsAttacking", false);

        ResumeMoving();
    }

   

    void HandleRanger(float distance)
    {
        if (isAttacking) return;

        if (distance <= shootingRange)
        {
            StopMoving();
            FacePlayer();

            if (Time.time >= lastAttackTime + attackCooldown)
                StartRangedAttack();
        }
        else if (distance <= detectionRange * 1.5f)
        {
            MoveTowardsPlayer();
        }
    }

    void StartRangedAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        StopMoving();

        if (animator != null)
            animator.SetTrigger("Shoot");

        Invoke(nameof(FireProjectile), 0.3f);
        Invoke(nameof(EndAttack), 1f);
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || shootPoint == null)
            return;

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

   

    void MoveTowardsPlayer()
    {
        if (navAgent != null && navAgent.enabled)
            navAgent.SetDestination(player.position);
    }

    void StopMoving()
    {
        if (navAgent != null)
            navAgent.isStopped = true;
    }

    void ResumeMoving()
    {
        if (navAgent != null)
            navAgent.isStopped = false;
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

        AudioClip clip = enemyType == EnemyType.Melee
            ? detectMeleeSound
            : detectRangerSound;

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            detectionSoundPlayed = true;
        }
    }

    

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

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

        StopMoving();

        if (animator != null)
            animator.SetTrigger("Die");

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        if (navAgent != null)
            navAgent.enabled = false;

        playerStats.AddExperience(experienceReward);

        InventoryManager inv = FindFirstObjectByType<InventoryManager>();

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
