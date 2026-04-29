using System.Collections;
using UnityEngine;
using Photon.Pun;
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

    private PhotonView _pv;

    // ── Moving platform rider tracking ────────────────────────────────────
    private Vector3 _lastBlockPos;
    private Collider _blockCollider;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    void Start()
    {
        _pv = GetComponent<PhotonView>();
        movingBlock = transform.GetChild(0);
        point1 = transform.GetChild(1);
        point2 = transform.GetChild(2);

        movingBlock.position = point1.position;
        currentPoint = point1;
        targetPoint = point2;

        _lastBlockPos = movingBlock.position;
        _blockCollider = movingBlock.GetComponent<Collider>();
    }

    // ── Rider carry — runs every frame ────────────────────────────────────
    // ── Rider carry — runs every frame ────────────────────────────────────
    private void Update()
    {
        Vector3 delta = movingBlock.position - _lastBlockPos;
        _lastBlockPos = movingBlock.position;

        // ── ONLY carry while block is actively moving ─────────────────────
        // Without this, floating point residue after Lerp finishes still
        // calls cc.Move() and fights against the player's own input
        if (!isMoving || delta.sqrMagnitude < 0.00001f || _blockCollider == null) return;

        Bounds b = _blockCollider.bounds;
        float skin = 0.3f;

        Collider[] hits = Physics.OverlapBox(
            b.center + Vector3.up * (b.extents.y + skin),
            new Vector3(b.extents.x * 0.95f, skin, b.extents.z * 0.95f)
        );

        foreach (Collider hit in hits)
        {
            CharacterController cc = hit.GetComponentInParent<CharacterController>();
            if (cc == null) continue;

            PhotonView pv = cc.GetComponentInParent<PhotonView>();
            if (pv != null && pv.IsMine)
                cc.Move(delta);
        }
    }

    // ── IMovable ───────────────────────────────────────────────────────────
    public bool CanPush(Vector3 casterPosition)
    {
        return IsBetweenXZ(casterPosition, targetPoint.position, movingBlock.position);
    }

    public bool CanPull(Vector3 casterPosition)
    {
        return IsBetweenXZ(currentPoint.position, casterPosition, targetPoint.position);
    }

    public void Push(Vector3 casterPosition)
    {
        if (!CanPush(casterPosition) || isMoving) return;
        _pv.RPC(nameof(RPC_Move), RpcTarget.All, targetPoint == point2);
    }

    public void Pull(Vector3 casterPosition)
    {
        if (!CanPull(casterPosition) || isMoving) return;
        _pv.RPC(nameof(RPC_Move), RpcTarget.All, targetPoint == point2);
    }

    // ── RPC — runs on every client ─────────────────────────────────────────
    [PunRPC]
    private void RPC_Move(bool toPoint2)
    {
        Transform destination = toPoint2 ? point2 : point1;
        Transform origin = toPoint2 ? point1 : point2;

        movingBlock.position = origin.position;
        currentPoint = origin;
        targetPoint = destination;

        StartCoroutine(MoveToPoint(destination));
    }

    // ── Internal helpers ───────────────────────────────────────────────────
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
        Vector3 startPos = movingBlock.position;

        while (elapsed < movementTime)
        {
            movingBlock.position = Vector3.Lerp(startPos, endPoint.position, elapsed / movementTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        movingBlock.position = endPoint.position;

        if (currentPoint == point1) { currentPoint = point2; targetPoint = point1; }
        else { currentPoint = point1; targetPoint = point2; }

        isMoving = false;
    }
}