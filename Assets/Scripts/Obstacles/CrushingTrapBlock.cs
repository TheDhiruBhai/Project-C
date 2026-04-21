using UnityEngine;
using Game.Player;

public class CrushingTrapBlock : MonoBehaviour
{
    private CrushingTrap trap;

    void Start()
    {
        trap = GetComponentInParent<CrushingTrap>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (trap != null) trap.OnBlockTriggerEnter(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (trap != null) trap.OnBlockTriggerExit(other);
    }
}
