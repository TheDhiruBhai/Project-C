using System.Collections;
using UnityEngine;
using Game.World;
using Game.Player;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;

public class SpikeTrap : Obstacle
{
    [SerializeField]
    private float damageAmount = 10f;
    [SerializeField]
    private float damageInterval = 1f;
    private HashSet<Health> playersInTrap = new HashSet<Health>();

    void OnTriggerEnter(Collider other)
    {
        //Checks if other collider is player and if it has a Health component, then starts damaging the player at intervals
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<Health>();

            //Checks if playerhealth is not null and if the hashset doesnt contain the player already
            if (playerHealth != null && playersInTrap.Add(playerHealth))
            {
                StartCoroutine(damagePlayer(playerHealth));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Checks if other collider is player and if it has a Health component
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<Health>();
            playersInTrap.Remove(playerHealth);
        }
    }

    IEnumerator damagePlayer(Health playerHealth)
    {
        while (playersInTrap.Contains(playerHealth))
        {
            var immune = playerHealth.GetComponent<Game.Player.PlayerInvulnerability>();
            if (immune == null || !immune.IsInvulnerable)
                playerHealth.TakeDamage((int)damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
