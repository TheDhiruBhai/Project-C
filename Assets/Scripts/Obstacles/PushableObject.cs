using System.Collections;
using UnityEngine;
using Game.World;

public class PushableObject : Obstacle, IMovable
{
    private Transform movingBlock;
    private Transform point1;
    private Transform point2;
    private Transform currentPoint;
    private Transform targetPoint;
    [SerializeField] private float movementTime = 1f;
    private bool isMoving = false;
    private PlayerController playerController;

    // ── Unity lifecycle ────────────────────────────────────────────────────

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

    // ── IMovable ───────────────────────────────────────────────────────────

    public bool CanPush(Vector3 casterPosition)
    {
        // Object must be between caster and its other point
        return IsBetweenXZ(casterPosition, targetPoint.position, movingBlock.position);
    }

    public bool CanPull(Vector3 casterPosition)
    {
        // Caster must be between the object's current and target point
        return IsBetweenXZ(currentPoint.position, casterPosition, targetPoint.position);
    }

    public void Push(Vector3 casterPosition)
    {
        if (!CanPush(casterPosition) || isMoving) return;
        StartCoroutine(MoveToPoint(targetPoint));
    }

    public void Pull(Vector3 casterPosition)
    {
        if (!CanPull(casterPosition) || isMoving) return;
        StartCoroutine(MoveToPoint(targetPoint));
    }

    // ── Internal helpers ───────────────────────────────────────────────────

    ///Returns true if middlePoint lies between pointA and pointB on the X and Z axes.
    private static bool IsBetweenXZ(Vector3 pointA, Vector3 pointB, Vector3 middlePoint)
    {
        return middlePoint.x >= Mathf.Min(pointA.x, pointB.x) &&
               middlePoint.x <= Mathf.Max(pointA.x, pointB.x) &&
               middlePoint.z >= Mathf.Min(pointA.z, pointB.z) &&
               middlePoint.z <= Mathf.Max(pointA.z, pointB.z);
    }

    private IEnumerator MoveToPoint(Transform endPoint)
    {
        isMoving = true;
        float elapsed = 0f;

        while (elapsed < movementTime)
        {
            float t = elapsed / movementTime;
            movingBlock.position = Vector3.Lerp(currentPoint.position, endPoint.position, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        movingBlock.position = endPoint.position;

        // Swap current ↔ target
        if (currentPoint == point1) { currentPoint = point2; targetPoint = point1; }
        else { currentPoint = point1; targetPoint = point2; }

        isMoving = false;
    }
}