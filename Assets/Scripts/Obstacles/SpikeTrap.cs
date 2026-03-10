using System.Collections;
using UnityEditor.UI;
using UnityEngine;

public class SpikeTrap : Obstacle
{
    private Transform spikeBed;
    private Transform endPoint;
    private Transform startPoint;
    [SerializeField]
    private float spikeTime = .5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spikeBed = transform.GetChild(0);
        endPoint = transform.GetChild(1);
        startPoint = transform;
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

    IEnumerator Activate() {
        spikeTime = .5f;
        while (spikeTime > 0) {
            spikeBed.position = Vector3.MoveTowards(spikeBed.position, endPoint.position, Time.deltaTime);
            yield return null;

            spikeTime -= Time.deltaTime;
        }
        
    }

    IEnumerator Deactivate() 
    {
        spikeTime = .5f;
        while (spikeTime > 0)
        {
            spikeBed.position = Vector3.MoveTowards(spikeBed.position, startPoint.position, Time.deltaTime);
            yield return null;

            spikeTime -= Time.deltaTime;
        }
    }
}
