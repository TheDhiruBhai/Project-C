using UnityEngine;

namespace Game.World
{
    public sealed class TransformableLiquid : MonoBehaviour, ITransformableLiquid
    {
        private Transform solidBlock;
        private Transform liquidBlock;

        [SerializeField] private bool frozen = false;
        public bool IsFrozen => frozen;

        public void Freeze()
        {
            if (frozen) return;
            frozen = true;
            solidBlock.gameObject.SetActive(true);
            liquidBlock.gameObject.SetActive(false);
        }

        public void Melt()
        {
            if (!frozen) return;
            frozen = false;
            solidBlock.gameObject.SetActive(false);
            liquidBlock.gameObject.SetActive(true);
        }

        private void Start()
        {
            solidBlock = transform.GetChild(0);
            liquidBlock = transform.GetChild(1);

            // Reflect starting state
            solidBlock.gameObject.SetActive(frozen);
            liquidBlock.gameObject.SetActive(!frozen);
        }
    }
}
