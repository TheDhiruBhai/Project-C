using UnityEngine;

namespace Game.World
{
    public sealed class SteelGate : MonoBehaviour
    {
        // No runtime logic needed — collision is handled entirely by Unity's physics layer matrix. 

        [SerializeField]
        [Tooltip("Visual mesh of the gate — can be referenced here if you need to " +
                 "change material to indicate Watery Form is active nearby.")]
        private Renderer gateRenderer;

        // Optional: flash the gate material when a player with Watery Form is near.
        // Expand here if visual feedback is needed.
    }
}
