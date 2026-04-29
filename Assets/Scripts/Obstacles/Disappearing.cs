using System.Collections;
using UnityEngine;
using Game.World;
using Photon.Pun;

// PHOTON SYNC STRATEGY: "RPC Trigger — All Clients Run Locally"
// Any client can trigger HoldStill — RPC fires on all.
// The fade coroutine runs independently on every client with the same duration
// so all clients stay visually in sync without streaming alpha values.
//
// SETUP:
//   • Add PhotonView to this GameObject
//   • Remove IPunObservable from Observed Components (no longer needed)
// ─────────────────────────────────────────────────────────────────────────────
public class Disappearing : Obstacle, IHoldable
{
    private Material material;
    private Color startColor;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    [SerializeField] private float fadeDuration = 5f;

    private float _holdRemaining = 0f;
    private bool _isHeld = false;

    public bool IsHeld => _holdRemaining > 0f;

    // ── IHoldable ─────────────────────────────────────────────────────────
    // Any client calls this — RPC fires on ALL clients simultaneously
    public void HoldStill(float seconds) =>
        photonView.RPC(nameof(RPC_HoldStill), RpcTarget.All, seconds);

    [PunRPC]
    private void RPC_HoldStill(float seconds)
    {
        _holdRemaining = Mathf.Max(_holdRemaining, seconds);
        _isHeld = true;

        StopAllCoroutines();

        // Reset to fully visible on every client
        Color c = startColor;
        c.a = startColor.a;
        material.color = c;
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }

    // ─────────────────────────────────────────────────────────────────────
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        material = meshRenderer.material;
        startColor = material.color;

        // All clients start the fade loop — no MasterClient gate needed
        StartCoroutine(FadeOut(material, fadeDuration));
    }

    private void Update()
    {
        if (_holdRemaining <= 0f) return;

        _holdRemaining -= Time.deltaTime;

        if (_holdRemaining <= 0f)
        {
            _holdRemaining = 0f;
            _isHeld = false;

            // All clients restart the loop locally — same result, stays in sync
            StartCoroutine(FadeOut(material, fadeDuration));
        }
    }

    // ── Coroutines — run on EVERY client ─────────────────────────────────

    public IEnumerator FadeOut(Material mat, float duration)
    {
        boxCollider.enabled = false;
        float time = 0f;

        while (time < duration)
        {
            if (_isHeld) yield break;

            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, time / duration);
            mat.color = c;
            time += Time.deltaTime;
            yield return null;
        }

        Color final = startColor;
        final.a = 0f;
        mat.color = final;
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(2f);

        if (!_isHeld)
            StartCoroutine(FadeIn(mat, fadeDuration));
    }

    public IEnumerator FadeIn(Material mat, float duration)
    {
        meshRenderer.enabled = true;

        Color c = startColor;
        c.a = 0f;
        mat.color = c;
        float time = 0f;

        while (time < duration)
        {
            if (_isHeld) yield break;

            c.a = Mathf.Lerp(0f, startColor.a, time / duration);
            mat.color = c;
            time += Time.deltaTime;
            yield return null;
        }

        c.a = startColor.a;
        mat.color = c;
        boxCollider.enabled = true;

        yield return new WaitForSeconds(2f);

        if (!_isHeld)
            StartCoroutine(FadeOut(mat, fadeDuration));
    }
}