using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private int healAmount = 5;
    [SerializeField]
    private float healInterval = 1f;
    private HashSet<Health> playersInBlood = new HashSet<Health>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        //Checks if other collider is player and if it has a Health component, then starts damaging the player at intervals
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<Health>();

            //Checks if playerhealth is not null and if the hashset doesnt contain the player already
            if (playerHealth != null && playersInBlood.Add(playerHealth))
            {
                StartCoroutine(HealPlayer(playerHealth));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Checks if other collider is player and if it has a Health component
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<Health>();
            playersInBlood.Remove(playerHealth);


        }
    }

    IEnumerator HealPlayer(Health playerHealth) {
        while (playersInBlood.Contains(playerHealth))
        {
            playerHealth.Heal(healAmount);
            yield return new WaitForSeconds(healInterval);
        }
    }
}
