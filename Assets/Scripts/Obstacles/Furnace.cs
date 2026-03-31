using System.Collections;
using UnityEngine;
using Mono.Cecil;
using Game.World;

public class Furnace : Obstacle, IFlammable
{
    private MovingPlatform movingPlatform;
    private GameObject engine;
    private GameObject pipe;
    [SerializeField] private Material fireMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private float activationDelay = 0.5f;

    // ── Unity lifecycle ────────────────────────────────────────────────────

    void Start()
    {
        movingPlatform = transform.GetChild(0).GetComponent<MovingPlatform>();
        engine = transform.GetChild(1).gameObject;
        pipe = transform.GetChild(2).gameObject;
    }

    // ── Public API (called by levers, card abilities, or UnityEvents) ──────

    public void Ignite(float seconds) => TurnOn();
    public void TurnOn() => StartCoroutine(ActivateCoroutine());
    public void TurnOff() => StartCoroutine(DeactivateCoroutine());

    // ── Coroutines ─────────────────────────────────────────────────────────

    IEnumerator ActivateCoroutine()
    {
        yield return new WaitForSeconds(activationDelay);
        if (movingPlatform != null) movingPlatform.Activate();
        pipe.GetComponent<MeshRenderer>().material = fireMaterial;
        engine.GetComponent<MeshRenderer>().material = fireMaterial;
    }

    IEnumerator DeactivateCoroutine()
    {
        yield return new WaitForSeconds(activationDelay);
        if (movingPlatform != null) movingPlatform.Deactivate();
        pipe.GetComponent<MeshRenderer>().material = offMaterial;
        engine.GetComponent<MeshRenderer>().material = offMaterial;
    }
}
