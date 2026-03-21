using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class CrushingTrap : Obstacle
{


    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Deactivate();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Activate();
        }
    }

    void Activate() {
        anim.speed = 1f;
    }

    void Deactivate() {
        anim.speed = 0f;
    }
}
