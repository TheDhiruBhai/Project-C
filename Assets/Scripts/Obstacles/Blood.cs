using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "Local Player Authority"
// Same pattern as Poison — each client only heals its own local player.
// SETUP: Add a PhotonView component to this GameObject in the Inspector.
public class Blood : MonoBehaviourPun
{
    [SerializeField] private int healAmount = 5;
    [SerializeField] private float healInterval = 1f;
    private HashSet<Health> playersInBlood = new HashSet<Health>();

    void Start() { }
    void Update() { }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // PHOTON: Only process the local player
        var pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        var playerHealth = other.GetComponent<Health>();
        if (playerHealth != null && playersInBlood.Add(playerHealth))
            StartCoroutine(HealPlayer(playerHealth));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pv = other.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        var playerHealth = other.GetComponent<Health>();
        playersInBlood.Remove(playerHealth);
    }

    IEnumerator HealPlayer(Health playerHealth)
    {
        while (playersInBlood.Contains(playerHealth))
        {
            playerHealth.Heal(healAmount);
            yield return new WaitForSeconds(healInterval);
        }
    }
}
