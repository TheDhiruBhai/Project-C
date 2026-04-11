using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "Local Player Authority"
// OnTriggerEnter fires on every client, but we only damage the player whose
// PhotonView.IsMine == true. This means each client only handles its own player,
// so TakeDamage is never called twice for the same player.
// SETUP: Add a PhotonView component to this GameObject in the Inspector.
public class Poison : MonoBehaviourPun
{
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private float damageInterval = 1f;
    private HashSet<Health> playersInPoison = new HashSet<Health>();

    void Start() { }
    void Update() { }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // PHOTON: Skip any player that doesn't belong to this client
        var pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        var playerHealth = other.GetComponent<Health>();
        if (playerHealth != null && playersInPoison.Add(playerHealth))
            StartCoroutine(DamagePlayer(playerHealth));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        var playerHealth = other.GetComponent<Health>();
        playersInPoison.Remove(playerHealth);
    }

    IEnumerator DamagePlayer(Health playerHealth)
    {
        while (playersInPoison.Contains(playerHealth))
        {
            playerHealth.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
