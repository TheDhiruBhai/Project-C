using System.Collections;
using UnityEngine;
using Game.World;

public class MovingPlatform : Obstacle, IHoldable
{
    private Transform movingBlock;
    private Transform startPoint;
    private Transform endPoint;
    private bool atStart = true;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool isPaused = false;

    private float holdRemaining = 0f;

    // ── IHoldable ──────────────────────────────────────────────────────────

    public bool IsHeld => holdRemaining > 0f;

    public void HoldStill(float seconds)
    {
        holdRemaining = Mathf.Max(holdRemaining, seconds);
        isPaused = true;
    }

    // ── External control (used by Furnace) ─────────────────────────────────

    public void Activate() => isPaused = false;
    public void Deactivate() => isPaused = true;

    // ── Unity lifecycle ────────────────────────────────────────────────────

    void Start()
    {
        movingBlock = transform.GetChild(0);
        startPoint = transform.GetChild(1);
        endPoint = transform.GetChild(2);
        StartCoroutine(Travel());
    }

    void Update()
    {
        if (holdRemaining > 0f)
        {
            holdRemaining -= Time.deltaTime;
            if (holdRemaining <= 0f)
            {
                holdRemaining = 0f;
                isPaused = false;
            }
        }
    }

    // ── Coroutine ──────────────────────────────────────────────────────────

    public IEnumerator Travel()
    {
        Transform target = atStart ? endPoint : startPoint;

        while (Vector3.Distance(movingBlock.position, target.position) >= 0.1f)
        {
            while (isPaused) yield return null;
            movingBlock.position = Vector3.MoveTowards(
                movingBlock.position, target.position, speed * Time.deltaTime);
            yield return null;
        }

        atStart = !atStart;
        yield return new WaitForSeconds(2f);
        StartCoroutine(Travel());
    }
}
