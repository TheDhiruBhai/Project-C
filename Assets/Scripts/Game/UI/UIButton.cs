using UnityEngine;

public class UIButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject objectAttachedToButton;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowObject() { 
        objectAttachedToButton.SetActive(true);
    }
}
