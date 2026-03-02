using UnityEngine;

namespace Game.World
{
    public sealed class TransformableLiquid : MonoBehaviour, ITransformableLiquid
    {
        [SerializeField] private bool frozen;
        public bool IsFrozen => frozen;

        public void Freeze()
        {
            frozen = true;
            // Swap material, collider, and movement friction here.
        }

        public void Melt()
        {
            frozen = false;
        }
    }
}
