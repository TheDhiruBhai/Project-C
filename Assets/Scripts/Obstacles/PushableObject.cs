using System.Collections;
using UnityEngine;

public class PushableObject : Obstacle
{
    private Transform movingBlock;
    private Transform point1;
    private Transform point2;
    private PlayerController playerController;
    private Transform currentPoint;
    private Transform targetPoint;
    [SerializeField]
    private float movementTime = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        movingBlock = transform.GetChild(0);
        point1 = transform.GetChild(1);
        point2 = transform.GetChild(2);
        movingBlock.position = point1.position;
        currentPoint = point1;
        targetPoint = point2;
    }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Push();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Pull();
            }

        }

    void Activate() {
        //If push card is played
        Push();

        ////If pull card is played
        Pull();
    }

    void Push() {
        if (isBetween(playerController.transform, targetPoint, currentPoint)) { 
            StartCoroutine(MoveToPoint(targetPoint));
        }
    }

    void Pull() {

        if (isBetween(currentPoint, playerController.transform, targetPoint)){
            StartCoroutine(MoveToPoint(targetPoint));
        }
    }

    bool isBetween(Transform pointA, Transform pointB, Transform middlePoint) {
        return (
            (middlePoint.position.x >= Mathf.Min(pointA.position.x, pointB.position.x)) &&
            (middlePoint.position.x <= Mathf.Max(pointA.position.x, pointB.position.x)) &&
            (middlePoint.position.z >= Mathf.Min(pointA.position.z, pointB.position.z)) &&
            (middlePoint.position.z <= Mathf.Max(pointA.position.z, pointB.position.z))
            );
    }

    IEnumerator MoveToPoint(Transform endPoint) { 
        movementTime = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < movementTime) {
            float t = elapsedTime / movementTime;
            movingBlock.position = Vector3.Lerp(currentPoint.position, endPoint.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        movingBlock.position = endPoint.position;

        if (currentPoint == point1) {
            currentPoint = point2;
            targetPoint = point1;
        } else {
            currentPoint = point1;
            targetPoint = point2;
        }
    }

}
