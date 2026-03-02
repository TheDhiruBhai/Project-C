using UnityEngine;

namespace Game.World
{
    public sealed class Holdable : MonoBehaviour, IHoldable
    {
        private float _holdRemaining;
        public void HoldStill(float seconds)
        {
            _holdRemaining = Mathf.Max(_holdRemaining, seconds);
        }

        private void Update()
        {
            if (_holdRemaining <= 0f) return;
            _holdRemaining -= Time.deltaTime;

        }

        public bool IsHeld => _holdRemaining > 0f;
    }
}
