using UnityEngine;

public class InfectionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerStats player =
            other.GetComponent<PlayerStats>();

        if (player == null)
            return;

        player.currentInfection =
            player.maxInfection;

        Debug.Log("MAX INFECTION!");
    }
}