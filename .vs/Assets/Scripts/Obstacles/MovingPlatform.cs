using System.Collections;
using UnityEngine;
using Game.World;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "Master Client Authority + IPunObservable"
// Only the Master Client runs the Travel coroutine and moves the platform.
// The moving block position is streamed to all other clients every network tick.
// HoldStill, Activate, and Deactivate are broadcast via RPC.
// SETUP: Add a PhotonView component. In its Observed Components list, add this script.
public class MovingPlatform : Obstacle, IHoldable, IPunObservable
{
    private Transform movingBlock;
    private Transform startPoint;
    private Transform endPoint;
    private bool atStart = true;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool isPaused = false;

    private float holdRemaining = 0f;

    // Received position for non-master clients
    private Vector3 netBlockPosition;

    public bool IsHeld => holdRemaining > 0f;

    // PHOTON: Broadcast pause/resume to all clients
    public void HoldStill(float seconds) =>
        photonView.RPC("RPC_HoldStill", RpcTarget.All, seconds);

    public void Activate()   => photonView.RPC("RPC_SetPaused", RpcTarget.All, false);
    public void Deactivate() => photonView.RPC("RPC_SetPaused", RpcTarget.All, true);

    [PunRPC] private void RPC_HoldStill(float seconds)
    {
        holdRemaining = Mathf.Max(holdRemaining, seconds);
        isPaused = true;
    }
    [PunRPC] private void RPC_SetPaused(bool value) => isPaused = value;

    void Start()
    {
        movingBlock      = transform.GetChild(0);
        startPoint       = transform.GetChild(1);
        endPoint         = transform.GetChild(2);
        netBlockPosition = movingBlock.position;

        // PHOTON: Only Master Client drives the platform movement
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(Travel());
    }

    void Update()
    {
        if (holdRemaining > 0f)
        {
            holdRemaining -= Time.deltaTime;
            if (holdRemaining <= 0f) { holdRemaining = 0f; isPaused = false; }
        }

        // PHOTON: Non-master clients interpolate to the received position
        if (!PhotonNetwork.IsMasterClient)
            movingBlock.position = Vector3.Lerp(movingBlock.position, netBlockPosition, Time.deltaTime * 15f);
    }

    // ── IPunObservable ───────────────────────────────────────────────────

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(movingBlock.position);
        else
            netBlockPosition = (Vector3)stream.ReceiveNext();
    }

    // ── Coroutine (Master Client only) ───────────────────────────────────

    public IEnumerator Travel()
    {
        Transform target = atStart ? endPoint : startPoint;

        while (Vector3.Distance(movingBlock.position, target.position) >= 0.1f)
        {
            while (isPaused) yield return null;
            movingBlock.position = Vector3.MoveTowards(movingBlock.position, target.position, speed * Time.deltaTime);
            yield return null;
        }

        atStart = !atStart;
        yield return new WaitForSeconds(2f);
        StartCoroutine(Travel());
    }
}
