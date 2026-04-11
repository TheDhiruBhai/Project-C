using UnityEngine;

namespace Game.Player
{
    public sealed class PlayerSpeedModifier : MonoBehaviour, World.ISpeedModifiable
    {
        public float CurrentMultiplier { get; private set; } = 1f;

        private float _targetMultiplier = 1f;
        private float _remaining;

        public void ApplySpeedMultiplier(float multiplier, float duration)
        {
            // Take the highest multiplier if multiple cards are stacked
            if (multiplier > _targetMultiplier || _remaining <= 0f)
            {
                _targetMultiplier = multiplier;
            }
            _remaining = Mathf.Max(_remaining, duration);
            CurrentMultiplier = _targetMultiplier;
        }

        private void Update()
        {
            if (_remaining <= 0f) return;
            _remaining -= Time.deltaTime;
            if (_remaining <= 0f)
            {
                _remaining = 0f;
                _targetMultiplier = 1f;
                CurrentMultiplier = 1f;
            }
        }
    }
}