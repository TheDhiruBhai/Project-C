using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowObject(GameObject UIobject) { 
            UIobject.SetActive(true);
    }

    public void HideObject(GameObject UIobject) {
        UIobject.SetActive(false);

    }
}
