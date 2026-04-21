using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.World;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using Game.Player;


public class CrushingTrap : Obstacle, IHoldable
{
    private Animator anim;
    [SerializeField] private float crushTime = 3f;
    private bool isPaused = false;
    private float holdRemaining = 0f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageInterval = 1f;
    private HashSet<Health> playersCrushed = new HashSet<Health>();

    public bool IsHeld => holdRemaining > 0f;

    public void HoldStill(float seconds)
    {
        holdRemaining = Mathf.Max(holdRemaining, seconds);
        isPaused = true;
        anim.speed = 0f;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isCrushing", true);
        StartCoroutine(CycleTrap());
    }

    void Update()
    {
        if (holdRemaining > 0f)
        {
            holdRemaining -= Time.deltaTime;
            if (holdRemaining <= 0f)
            {
                holdRemaining = 0f;
                isPaused = false;
                anim.speed = 1f;
            }
        }
    }

    IEnumerator CycleTrap()
    {
        while (true)
        {
            // Crushing phase
            anim.SetBool("isCrushing", true);
            float timer = crushTime;
            while (timer > 0f)
            {
                if (!isPaused) timer -= Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            // Opening phase
            anim.SetBool("isCrushing", false);
            timer = crushTime;
            while (timer > 0f)
            {
                if (!isPaused) timer -= Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    public void OnBlockTriggerEnter(Collider other)
    {
        Debug.Log($"CrushingTrap trigger hit by {other.name} tag={other.tag}");
        if (!other.CompareTag("Player")) return;

        var playerHealth = other.GetComponent<Health>();
        if (playerHealth != null && playersCrushed.Add(playerHealth))
            StartCoroutine(DamagePlayer(playerHealth));
    }

    public void OnBlockTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerHealth = other.GetComponent<Health>();
        playersCrushed.Remove(playerHealth);
    }

    IEnumerator DamagePlayer(Health playerHealth)
    {
        while (playersCrushed.Contains(playerHealth))
        {
            playerHealth.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
