using System.Collections;
using UnityEngine;

public class MovingPlatform : Obstacle
{

    private Transform startPoint;
    private Transform endPoint;
    private bool atStart = true;
    private Transform movingBlock;
    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movingBlock = transform.GetChild(0);
        startPoint = transform.GetChild(1);
        endPoint = transform.GetChild(2);
        StartCoroutine(Travel());
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

    public void Activate() {
        isPaused = false;
    }

    public void Deactivate() { 
        isPaused = true;
    }

    public IEnumerator Travel()
    {
        if (atStart)
        {
            while (Vector3.Distance(movingBlock.position, endPoint.position) >= .1)
            {
                while (isPaused)
                {
                    yield return null;
                }
                movingBlock.position = Vector3.MoveTowards(movingBlock.position, endPoint.position, speed *Time.deltaTime);
                yield return null;
            }
            atStart = false;
            yield return new WaitForSeconds(2f);
            StartCoroutine(Travel());
        }
        else
        {
            while (Vector3.Distance(movingBlock.position, startPoint.position) >= .1)
            {
                while (isPaused)
                {
                    yield return null;
                }
                movingBlock.position = Vector3.MoveTowards(movingBlock.position, startPoint.position, speed * Time.deltaTime);
                yield return null;
            }
            atStart = true;
            yield return new WaitForSeconds(2f);
            StartCoroutine(Travel());
        }
    }
}
