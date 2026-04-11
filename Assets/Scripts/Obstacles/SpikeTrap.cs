using System.Collections;
using UnityEngine;
using Game.World;
using Game.Player;
using System.Collections.Generic;

// PHOTON SYNC STRATEGY: "Local Player Authority"
public class SpikeTrap : MonoBehaviour

{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float damageInterval = 1f;
    private HashSet<Health> playersInTrap = new HashSet<Health>();

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerHealth = other.GetComponent<Health>();
        if (playerHealth != null && playersInTrap.Add(playerHealth))
            StartCoroutine(DamagePlayer(playerHealth));
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerHealth = other.GetComponent<Health>();
        playersInTrap.Remove(playerHealth);
    }

    IEnumerator DamagePlayer(Health playerHealth)
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
