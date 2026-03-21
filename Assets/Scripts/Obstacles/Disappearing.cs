using System.Collections;
using UnityEngine;

public class Disappearing : Obstacle
{
    private Material material;
    private float fadeDuration = 5f;
    private Color startColor;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<Renderer>().material;
        startColor = material.color;
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        StartCoroutine(FadeOut(material, fadeDuration));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Activate();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Deactivate();
        }
    }

    void Activate() { 
        material.color = startColor;
        boxCollider.enabled = true;
        StopAllCoroutines();
    }

    void Deactivate() {
        StartCoroutine(FadeOut(material, fadeDuration));
    }


    public IEnumerator FadeOut(Material mat, float duration)
    {
        //Setting variables for the fade out process
        Color color = startColor;
        float startAlpha = color.a;
        float time = 0f;
        boxCollider.enabled = false;

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
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeIn(mat, fadeDuration));
    }

    public IEnumerator FadeIn(Material mat, float duration)
    {
        //Setting variables for the fade in process
        Color color = startColor;
        float endAlpha = color.a;
        color.a = 0f;
        mat.color = color;
        float time = 0f;
        while (time < duration)
        {
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
