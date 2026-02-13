using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 15;
    public int infectionAmount = 10;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();

        if (player != null)
        {
            player.TakeDamage(damage);
            player.AddInfection(infectionAmount);

            Destroy(gameObject);
            return;
        }

        
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}