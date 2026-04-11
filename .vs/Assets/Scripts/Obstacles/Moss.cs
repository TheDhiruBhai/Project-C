using System.Collections;
using UnityEngine;
using Game.World;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "RPC Broadcast"
// Ignite() broadcasts an RPC to ALL clients so everyone sees the moss burn away
// at the same time. AllBuffered ensures late-joiners also see the burned state.
// SETUP: Add a PhotonView component to this GameObject in the Inspector.
public class Moss : Obstacle, IFlammable
{
    private Material mossMaterial;
    private Color startColor;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    [SerializeField] private float fadeDuration = 3f;

    // PHOTON: Ignite now broadcasts to all clients.
    public void Ignite(float seconds) =>
        photonView.RPC("RPC_Ignite", RpcTarget.AllBuffered);

    [PunRPC]
    private void RPC_Ignite() =>
        StartCoroutine(MossFadeOut(mossMaterial, fadeDuration));

    void Start()
    {
        meshRenderer  = GetComponent<MeshRenderer>();
        boxCollider   = GetComponent<BoxCollider>();
        mossMaterial  = GetComponent<MeshRenderer>().material;
        startColor    = mossMaterial.color;
    }

    public IEnumerator MossFadeOut(Material mat, float duration)
    {
        Color color = startColor;
        float time  = 0f;

        while (time < duration)
        {
            color.a   = Mathf.Lerp(startColor.a, 0f, time / duration);
            mat.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a          = 0f;
        mat.color        = color;
        meshRenderer.enabled = false;
        boxCollider.enabled  = false;
    }
}
