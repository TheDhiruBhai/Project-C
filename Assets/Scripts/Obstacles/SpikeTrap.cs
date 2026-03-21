using Game.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class SpikeTrap : Obstacle
{
    [SerializeField]
    private float damageAmount = 10f;
    [SerializeField]
    private float damageInterval = 1f;
    private HashSet<Health> playersInTrap = new HashSet<Health>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            if(playerHealth != null && playersInTrap.Add(playerHealth))
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
            playersInTrap.Remove(playerHealth);
          
           
        }
    }



    IEnumerator damagePlayer(Health playerHealth) {
        while (playersInTrap.Contains(playerHealth))
        {
            playerHealth.TakeDamage((int)damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}

