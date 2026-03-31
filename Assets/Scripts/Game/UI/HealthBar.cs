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
        playerHealth = FindFirstObjectByType<PlayerController>().GetComponent<Health>();
        playerMaxHP = playerHealth.MaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Player Current HP: " + playerHealth.CurrentHp);

        healthBarFill.value = (float) playerHealth.CurrentHp/playerMaxHP;
    }
}
