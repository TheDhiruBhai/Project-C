using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private int damageAmount = 5;
    [SerializeField]
    private float damageInterval = 1f;
    private HashSet<Health> playersInPoison = new HashSet<Health>();
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
            if (playerHealth != null && playersInPoison.Add(playerHealth))
            {
                StartCoroutine(damagePlayer(playerHealth));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Checks if other collider is player and if it has a Health component
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<Health>();
            playersInPoison.Remove(playerHealth);


        }
    }

    IEnumerator damagePlayer(Health playerHealth)
    {
        while (playersInPoison.Contains(playerHealth))
        {
            playerHealth.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
