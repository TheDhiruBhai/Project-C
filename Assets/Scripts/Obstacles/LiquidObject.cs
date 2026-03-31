using UnityEngine;
using Game.World;

public class LiquidObject : Obstacle, ITransformableLiquid
{
    private Transform solidBlock;
    private Transform liquidBlock;
    [SerializeField] private bool isSolid = true; // starts as liquid

    // ── ITransformableLiquid ───────────────────────────────────────────────

    public bool IsFrozen => isSolid;

    public void Freeze()
    {
        if (isSolid) return;
        isSolid = true;
        solidBlock.gameObject.SetActive(true);
        liquidBlock.gameObject.SetActive(false);
    }

    public void Melt()
    {
        if (!isSolid) return;
        isSolid = false;
        solidBlock.gameObject.SetActive(false);
        liquidBlock.gameObject.SetActive(true);
    }

    // ── Unity lifecycle ────────────────────────────────────────────────────

    void Start()
    {
        solidBlock = transform.GetChild(0);
        liquidBlock = transform.GetChild(1);
    }
}
