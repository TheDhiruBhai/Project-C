using System.Collections;
using UnityEngine;
using Game.World;
using Game.Player;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;

public class SpikeTrap : Obstacle, IHoldable
{
    private Transform spikeBed;
    private Transform endPoint;
    private Transform startPoint;

    [SerializeField] private float spikeTime = 0.5f;
    [SerializeField] private float pauseTime = 2f;
    [SerializeField]
    private float damageAmount = 10f;
    [SerializeField]
    private float damageInterval = 1f;
    private HashSet<Health> playersInTrap = new HashSet<Health>();

    private float holdRemaining = 0f;
    private bool isHeld = false;
    private bool extended = false;

    // ── IHoldable ──────────────────────────────────────────────────────────

    public bool IsHeld => holdRemaining > 0f;

    public void HoldStill(float seconds)
    {
        holdRemaining = Mathf.Max(holdRemaining, seconds);
        isHeld = true;
        StopAllCoroutines();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!extended) return;
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
            playerHealth.TakeDamage((int)damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }


    // ── Coroutines ─────────────────────────────────────────────────────────

    IEnumerator SpikeLoop()
    {
        while (true)
        {
            yield return StartCoroutine(ExtendSpikes());
            yield return new WaitForSeconds(pauseTime);
            yield return StartCoroutine(RetractSpikes());
            yield return new WaitForSeconds(pauseTime);
        }
    }

    IEnumerator ExtendSpikes()
    {
        float timer = spikeTime;
        while (timer > 0f)
        {
            if (isHeld) yield break;
            spikeBed.position = Vector3.MoveTowards(
                spikeBed.position, endPoint.position, Time.deltaTime / spikeTime);
            timer -= Time.deltaTime;
            yield return null;
        }
        extended = true;
    }

    IEnumerator RetractSpikes()
    {
        extended = false;
        float timer = spikeTime;
        while (timer > 0f)
        {
            if (isHeld) yield break;
            spikeBed.position = Vector3.MoveTowards(
                spikeBed.position, startPoint.position, Time.deltaTime / spikeTime);
            timer -= Time.deltaTime;
            yield return null;
        }
    }
}
