using System.Collections;
using UnityEngine;
using Game.World;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;


public class CrushingTrap : Obstacle, IHoldable
{
    private Transform rightBlock;
    private Transform leftBlock;
    private Vector3 rightBlockStartPos;
    private Vector3 leftBlockStartPos;
    private Animator anim;
    [SerializeField] private float crushTime = 3f;
    private bool isPaused = false;
    private float holdRemaining = 0f;

    // ── IHoldable ──────────────────────────────────────────────────────────

    public bool IsHeld => holdRemaining > 0f;

    public void HoldStill(float seconds)
    {
        holdRemaining = Mathf.Max(holdRemaining, seconds);
        isPaused = true;
    }

    // ── Unity lifecycle ────────────────────────────────────────────────────

    void Start()
    {
        anim = GetComponent<Animator>();
        rightBlock = transform.GetChild(0);
        leftBlock = transform.GetChild(1);
        rightBlockStartPos = rightBlock.localPosition;
        leftBlockStartPos = leftBlock.localPosition;
        StartCoroutine(Crush());
    }

    void Update()
    {
        // Tick down the hold timer set by Stable Ground card
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

    // ── Coroutines ─────────────────────────────────────────────────────────

    IEnumerator Crush()
    {
        float timer = crushTime;
        while (timer > 0f)
        {
            while (isPaused) yield return null;

            rightBlock.localPosition = Vector3.MoveTowards(
                rightBlock.localPosition, Vector3.zero, Time.deltaTime);
            leftBlock.localPosition = Vector3.MoveTowards(
                leftBlock.localPosition, Vector3.zero, Time.deltaTime);

            timer -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        float timer = crushTime;
        while (timer > 0f)
        {
            while (isPaused) yield return null;

            rightBlock.localPosition = Vector3.MoveTowards(
                rightBlock.localPosition, rightBlockStartPos, Time.deltaTime);
            leftBlock.localPosition = Vector3.MoveTowards(
                leftBlock.localPosition, leftBlockStartPos, Time.deltaTime);

            timer -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(Crush());
    }
}
