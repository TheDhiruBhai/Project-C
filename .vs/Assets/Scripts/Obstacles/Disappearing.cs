using System.Collections;
using UnityEngine;
using Game.World;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "Master Client Authority + IPunObservable"
// Only the Master Client runs the FadeOut/FadeIn cycle.
// The material alpha and collider/renderer state are streamed to all other clients.
// HoldStill is broadcast via RPC.
// SETUP: Add a PhotonView component. In its Observed Components list, add this script.
public class Disappearing : Obstacle, IHoldable, IPunObservable
{
    private Material material;
    private Color startColor;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    [SerializeField] private float fadeDuration = 5f;

    private float holdRemaining = 0f;
    private bool isHeld = false;

    // Synced values received from Master Client
    private float netAlpha = 1f;
    private bool  netRendererEnabled  = true;
    private bool  netColliderEnabled  = true;

    public bool IsHeld => holdRemaining > 0f;

    // PHOTON: Broadcast hold to all clients
    public void HoldStill(float seconds) =>
        photonView.RPC("RPC_HoldStill", RpcTarget.All, seconds);

    [PunRPC]
    private void RPC_HoldStill(float seconds)
    {
        holdRemaining = Mathf.Max(holdRemaining, seconds);
        isHeld = true;
        StopAllCoroutines();
        material.color         = startColor;
        boxCollider.enabled    = true;
        meshRenderer.enabled   = true;
    }

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider  = GetComponent<BoxCollider>();
        material     = GetComponent<MeshRenderer>().material;
        startColor   = material.color;

        // PHOTON: Only Master Client drives the fade animation
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(FadeOut(material, fadeDuration));
    }

    void Update()
    {
        if (holdRemaining > 0f)
        {
            holdRemaining -= Time.deltaTime;
            if (holdRemaining <= 0f)
            {
                holdRemaining = 0f;
                isHeld = false;
                if (PhotonNetwork.IsMasterClient)
                    StartCoroutine(FadeOut(material, fadeDuration));
            }
        }

        // PHOTON: Non-master clients apply the synced visual state
        if (!PhotonNetwork.IsMasterClient)
        {
            Color c = material.color;
            c.a = Mathf.Lerp(c.a, netAlpha, Time.deltaTime * 10f);
            material.color         = c;
            meshRenderer.enabled   = netRendererEnabled;
            boxCollider.enabled    = netColliderEnabled;
        }
    }

    // ── IPunObservable ───────────────────────────────────────────────────

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(material.color.a);
            stream.SendNext(meshRenderer.enabled);
            stream.SendNext(boxCollider.enabled);
        }
        else
        {
            netAlpha            = (float)stream.ReceiveNext();
            netRendererEnabled  = (bool)stream.ReceiveNext();
            netColliderEnabled  = (bool)stream.ReceiveNext();
        }
    }

    // ── Coroutines (Master Client only) ─────────────────────────────────

    public IEnumerator FadeOut(Material mat, float duration)
    {
        Color color = startColor;
        float time  = 0f;
        boxCollider.enabled = false;

        while (time < duration)
        {
            if (isHeld) yield break;
            color.a   = Mathf.Lerp(startColor.a, 0f, time / duration);
            mat.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a          = 0f;
        mat.color        = color;
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeIn(mat, fadeDuration));
    }

    public IEnumerator FadeIn(Material mat, float duration)
    {
        Color color = startColor;
        color.a     = 0f;
        mat.color   = color;
        meshRenderer.enabled = true;
        float time  = 0f;

        while (time < duration)
        {
            if (isHeld) yield break;
            color.a   = Mathf.Lerp(0f, startColor.a, time / duration);
            mat.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a          = startColor.a;
        mat.color        = color;
        boxCollider.enabled = true;

        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeOut(mat, fadeDuration));
    }
}
