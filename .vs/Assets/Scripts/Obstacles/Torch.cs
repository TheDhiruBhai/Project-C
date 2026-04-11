using UnityEngine;
using Game.World;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "RPC Broadcast"
// Light() and Extinguish() broadcast an RPC to ALL clients so every player
// sees the same flame state at the same time.
// SETUP: Add a PhotonView component to this GameObject in the Inspector.
public class Torch : Obstacle, IFlammable
{
    private GameObject torchFlame;

    void Start()
    {
        torchFlame = transform.GetChild(1).gameObject;
        torchFlame.SetActive(false);
    }

    public void Ignite(float seconds) => Light();

    // PHOTON: These now send an RPC to all clients instead of running locally.
    public void Light()     => photonView.RPC("RPC_Light",     RpcTarget.AllBuffered);
    public void Extinguish() => photonView.RPC("RPC_Extinguish", RpcTarget.AllBuffered);

    // AllBuffered means late-joining players also receive the last state.
    [PunRPC] private void RPC_Light()      => torchFlame.SetActive(true);
    [PunRPC] private void RPC_Extinguish() => torchFlame.SetActive(false);
}
