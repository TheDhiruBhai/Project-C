using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class CrushingTrap : Obstacle
{
    private Transform rightBlock;
    private Transform leftBlock;
    [SerializeField]
    private float crushTime = 3f;
    private Vector3 rightBlockStartPos;
    private Vector3 leftBlockStartPos;
    private bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rightBlock = transform.GetChild(0);
        leftBlock = transform.GetChild(1);
        rightBlockStartPos = rightBlock.localPosition;
        leftBlockStartPos = leftBlock.localPosition;
        StartCoroutine(Crush());
        
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
        isPaused = false;
    }

    void Deactivate() {
        isPaused = true;
    }

    IEnumerator Crush()
    {
        crushTime = 3f;
        while (crushTime > 0)
        {
            while (isPaused)
            {
                yield return null;
            }
            rightBlock.localPosition = Vector3.MoveTowards(rightBlock.localPosition, new Vector3(0, 0, rightBlock.localPosition.z), Time.deltaTime);
            leftBlock.localPosition = Vector3.MoveTowards(leftBlock.localPosition,new Vector3(0, 0, rightBlock.localPosition.z), Time.deltaTime);

            crushTime -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        StopAllCoroutines();
        StartCoroutine(Open());

    }

    IEnumerator Open()
    {
        crushTime = 3f;
        while (crushTime > 0)
        {
            while (isPaused)
            {
                yield return null;
            }
            rightBlock.localPosition = Vector3.MoveTowards(rightBlock.localPosition, rightBlockStartPos, Time.deltaTime);
            leftBlock.localPosition = Vector3.MoveTowards(leftBlock.localPosition, leftBlockStartPos, Time.deltaTime);
            crushTime -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        StopAllCoroutines();
        StartCoroutine(Crush());

    }
}
