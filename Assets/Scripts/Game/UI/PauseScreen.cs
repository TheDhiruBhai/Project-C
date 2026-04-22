using UnityEngine;

public class PauseScreen : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        PlayerController.GrimoireControl += TogglePause;
    }

    private void OnDisable()
    {
        PlayerController.GrimoireControl -= TogglePause;
    }

    private void TogglePause() {

        Debug.Log("Toggling pause menu");
        if (isPaused)
        {
            pauseMenu.SetActive(false);
            isPaused = false;
        }
        else { 
            pauseMenu.SetActive(true);
            isPaused = true;
        }
    }
}
