using System.Collections;
using UnityEngine;
using Game.World;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "RPC Broadcast"
// TurnOn and TurnOff broadcast RPCs to ALL clients so everyone sees the same
// material change and the MovingPlatform activates on every machine.
// SETUP: Add a PhotonView component to this GameObject in the Inspector.
public class Furnace : Obstacle, IFlammable
{
    private MovingPlatform movingPlatform;
    private GameObject engine;
    private GameObject pipe;
    [SerializeField] private Material fireMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private float activationDelay = 0.5f;

    void Start()
    {
        movingPlatform = transform.GetChild(0).GetComponent<MovingPlatform>();
        engine         = transform.GetChild(1).gameObject;
        pipe           = transform.GetChild(2).gameObject;
    }

    // PHOTON: These now broadcast to all clients instead of running locally.
    public void Ignite(float seconds) => TurnOn();
    public void TurnOn()  => photonView.RPC("RPC_TurnOn",  RpcTarget.AllBuffered);
    public void TurnOff() => photonView.RPC("RPC_TurnOff", RpcTarget.AllBuffered);

    [PunRPC] private void RPC_TurnOn()  => StartCoroutine(ActivateCoroutine());
    [PunRPC] private void RPC_TurnOff() => StartCoroutine(DeactivateCoroutine());

    IEnumerator ActivateCoroutine()
    {
        yield return new WaitForSeconds(activationDelay);
        if (movingPlatform != null) movingPlatform.Activate();
        pipe.GetComponent<MeshRenderer>().material   = fireMaterial;
        engine.GetComponent<MeshRenderer>().material = fireMaterial;
    }

    IEnumerator DeactivateCoroutine()
    {
        yield return new WaitForSeconds(activationDelay);
        if (movingPlatform != null) movingPlatform.Deactivate();
        pipe.GetComponent<MeshRenderer>().material   = offMaterial;
        engine.GetComponent<MeshRenderer>().material = offMaterial;
    }
}
