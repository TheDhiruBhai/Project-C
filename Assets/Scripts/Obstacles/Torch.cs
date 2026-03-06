using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class Torch : Obstacle
{
    /* Torch
     * Torches have 2 states, lit and unlit, when activated, they permanently change to lit state, emitting light and allowing players to see in the darkness
     * 
     */

    private GameObject torchFlame;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torchFlame = transform.GetChild(1).gameObject;
        torchFlame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Activate();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Deactivate();
        }

        void Activate()
        {
            torchFlame.SetActive(true);
        }

        void Deactivate()
        {
            torchFlame.SetActive(false);
        }
    }
}
