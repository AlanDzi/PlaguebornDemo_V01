using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    public Animator animator;
    public float timeBetweenAttacks = 3.0f; // How many seconds between attacks
    private float timer;

    void Start()
    {
        // Initialize the timer
        timer = timeBetweenAttacks;
    }

    void Update()
    {
        // Count down the time
        timer -= Time.deltaTime;

        // If time is up (less or equal to 0)
        if (timer <= 0)
        {
            ChooseAttack();
            timer = timeBetweenAttacks; // Reset the timer
        }
    }

    void ChooseAttack()
    {
        // Pick a random number: 0 or 1
        int randomPick = Random.Range(0, 2);

        if (randomPick == 0)
        {
            animator.SetTrigger("Punch"); 
        }
        else
        {
            animator.SetTrigger("Swipe"); 
        }
    }
}