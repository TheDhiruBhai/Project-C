using System.Collections;
using UnityEngine;
using Game.World;

public class Moss : Obstacle, IFlammable
{
    private Material mossMaterial;
    private Color startColor;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    [SerializeField] private float fadeDuration = 3f;

    // ── IFlammable ─────────────────────────────────────────────────────────

    public void Ignite(float seconds)
    {
        StartCoroutine(MossFadeOut(mossMaterial, fadeDuration));
    }

    // ── Unity lifecycle ────────────────────────────────────────────────────

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        mossMaterial = GetComponent<Renderer>().material;
        startColor = mossMaterial.color;
    }

    // ── Coroutine ──────────────────────────────────────────────────────────

    public IEnumerator MossFadeOut(Material mat, float duration)
    {
        Color color = startColor;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            color.a = Mathf.Lerp(startAlpha, 0f, t);
            mat.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        mat.color = color;
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
    }
}