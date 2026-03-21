using Game.Player;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    private Health playerHealth;
    [SerializeField]
    private Slider healthBarFill;
    private int playerMaxHP;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerController>().GetComponent<Health>();
        playerMaxHP = playerHealth.GetMaxHP();



    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Player Current HP: " + playerHealth.GetCurrentHP());

        healthBarFill.value = (float) playerHealth.GetCurrentHP()/playerMaxHP;
    }
}
