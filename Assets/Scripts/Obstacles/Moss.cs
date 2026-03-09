using System.Collections;
using UnityEngine;

public class Moss : Obstacle
{

    [SerializeField]
    private Material mossMaterial;
    private float fadeDuration = 3f;
    private Color startColor;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mossMaterial = GetComponent<Renderer>().material;
        startColor = mossMaterial.color;
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            Activate();
        }
        else if(Input.GetMouseButtonDown(1)) {
            Deactivate();
        }
    }

    void Activate() { 
        StartCoroutine(MossFadeOut(mossMaterial, fadeDuration));
    }

    void Deactivate() {
        StopAllCoroutines();
        mossMaterial.color = startColor;
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }

    public IEnumerator MossFadeOut(Material mat, float duration) {
        //Setting variables for the fade out process
        Color color = startColor;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration) {
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
