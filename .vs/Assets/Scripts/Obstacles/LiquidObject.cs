using UnityEngine;
using Game.World;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "RPC Broadcast"
// Freeze() and Melt() broadcast RPCs to ALL clients so every player sees the
// same solid/liquid state at the same time.
// AllBuffered means late-joining players also receive the current state.
// SETUP: Add a PhotonView component to this GameObject in the Inspector.
public class LiquidObject : Obstacle, ITransformableLiquid
{
    private Transform solidBlock;
    private Transform liquidBlock;
    [SerializeField] private bool isSolid = true;

    public bool IsFrozen => isSolid;

    // PHOTON: These now broadcast to all clients.
    public void Freeze() => photonView.RPC("RPC_Freeze", RpcTarget.AllBuffered);
    public void Melt()   => photonView.RPC("RPC_Melt",   RpcTarget.AllBuffered);

    [PunRPC]
    private void RPC_Freeze()
    {
        if (isSolid) return;
        isSolid = true;
        solidBlock.gameObject.SetActive(true);
        liquidBlock.gameObject.SetActive(false);
    }

    [PunRPC]
    private void RPC_Melt()
    {
        if (!isSolid) return;
        isSolid = false;
        solidBlock.gameObject.SetActive(false);
        liquidBlock.gameObject.SetActive(true);
    }

    void Start()
    {
        solidBlock  = transform.GetChild(0);
        liquidBlock = transform.GetChild(1);
    }
}
