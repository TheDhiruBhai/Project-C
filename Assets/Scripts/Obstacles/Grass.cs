using System.Collections.Generic;
using UnityEngine;

public class Grass : Obstacle
{

    private GameObject plantBridge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plantBridge = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) )
        { 
            Activate();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Deactivate();
        }
    }

    void Activate() { 

        //When cards are implemented, this should check if player hits growable reciever while standing on growable sender
        plantBridge.SetActive(true);
    }

    void Deactivate() { 
        plantBridge.SetActive(false);
    }
}
