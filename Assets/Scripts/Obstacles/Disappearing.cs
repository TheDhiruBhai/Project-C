using System.Collections;
using UnityEngine;
using Game.World;

public class Disappearing : Obstacle, IHoldable
{
    private Material material;
    private Color startColor;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    [SerializeField] private float fadeDuration = 5f;

    private float holdRemaining = 0f;
    private bool isHeld = false;

    // ── IHoldable ──────────────────────────────────────────────────────────

    public bool IsHeld => holdRemaining > 0f;

    public void HoldStill(float seconds)
    {
        holdRemaining = Mathf.Max(holdRemaining, seconds);
        isHeld = true;

        // Snap to fully visible and re-enable collider while held
        StopAllCoroutines();
        material.color = startColor;
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
    }

    // ── Unity lifecycle ────────────────────────────────────────────────────

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponent<Renderer>().material;
        startColor = material.color;
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
                // Resume the fade cycle
                StartCoroutine(FadeOut(material, fadeDuration));
            }
        }
    }

    // ── Coroutines ─────────────────────────────────────────────────────────

    public IEnumerator FadeOut(Material mat, float duration)
    {
        Color color = startColor;
        float startAlpha = color.a;
        float time = 0f;
        boxCollider.enabled = false;

        while (time < duration)
        {
            if (isHeld) yield break; // Abort if Stable Ground interrupts
            float t = time / duration;
            color.a = Mathf.Lerp(startAlpha, 0f, t);
            mat.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        mat.color = color;
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeIn(mat, fadeDuration));
    }

    public IEnumerator FadeIn(Material mat, float duration)
    {
        Color color = startColor;
        float endAlpha = color.a;
        color.a = 0f;
        mat.color = color;
        meshRenderer.enabled = true;
        float time = 0f;

        while (time < duration)
        {
            if (isHeld) yield break; // Abort if Stable Ground interrupts
            float t = time / duration;
            color.a = Mathf.Lerp(0f, endAlpha, t);
            mat.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        mat.color = color;
        boxCollider.enabled = true;

        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeOut(mat, fadeDuration));
    }
}
