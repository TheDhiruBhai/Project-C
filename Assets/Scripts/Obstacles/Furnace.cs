using Mono.Cecil;
using System.Collections;
using UnityEngine;

public class Furnace : Obstacle
{

    private MovingPlatform movingPlatform;
    private GameObject engine;
    private GameObject pipe;
    [SerializeField]
    private Material fire;
    [SerializeField]
    private Material startColor;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movingPlatform = transform.GetChild(0).GetComponent<MovingPlatform>();
        engine = transform.GetChild(1).gameObject;
        pipe = transform.GetChild(2).gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Activate());
        }
        else if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Deactivate());
        }
    }

    public IEnumerator Activate()
    {
        yield return new WaitForSeconds(.5f);
        movingPlatform.Activate();
        pipe.GetComponent<MeshRenderer>().material = fire;
        engine.GetComponent<MeshRenderer>().material = fire;    
    }

    public IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(.5f);
        movingPlatform.Deactivate();
        pipe.GetComponent<MeshRenderer>().material = startColor;
        engine.GetComponent<MeshRenderer>().material = startColor;
    }
}
